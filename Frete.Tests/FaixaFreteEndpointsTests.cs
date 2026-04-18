using System.Net;
using System.Net.Http.Json;
using Frete.Application.DTOs;

namespace Frete.Tests;

public sealed class FaixaFreteEndpointsTests : IClassFixture<FreteApiFactory>
{
    private readonly FreteApiFactory _factory;

    public FaixaFreteEndpointsTests(FreteApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveListarESubstituirFaixas()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");

        var tabela = await CriarTabelaFreteAsync(client, "FX");
        var incidencia = await CriarIncidenciaAsync(client, tabela.Id);

        var vazias = await client.GetFromJsonAsync<List<FaixaFreteResponse>>(
            $"/api/v1/tabelas-frete-cliente/{incidencia.Id}/faixas");

        Assert.NotNull(vazias);
        Assert.Empty(vazias!);

        var payload = new[]
        {
            new FaixaFreteItemRequest { LimiteInferiorKg = 0, LimiteSuperiorKg = 10, Valor = 100 },
            new FaixaFreteItemRequest { LimiteInferiorKg = 11, LimiteSuperiorKg = 20, Valor = 180 }
        };

        var putResponse = await client.PutAsJsonAsync(
            $"/api/v1/tabelas-frete-cliente/{incidencia.Id}/faixas",
            payload);

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
        var salvas = await putResponse.Content.ReadFromJsonAsync<List<FaixaFreteResponse>>();
        Assert.NotNull(salvas);
        Assert.Equal(2, salvas!.Count);
        Assert.Equal(11, salvas[1].LimiteInferiorKg);

        var listadas = await client.GetFromJsonAsync<List<FaixaFreteResponse>>(
            $"/api/v1/tabelas-frete-cliente/{incidencia.Id}/faixas");

        Assert.NotNull(listadas);
        Assert.Equal(2, listadas!.Count);
    }

    [Fact]
    public async Task DeveRejeitarFaixasSobrepostas()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");

        var tabela = await CriarTabelaFreteAsync(client, "OV");
        var incidencia = await CriarIncidenciaAsync(client, tabela.Id);

        var payload = new[]
        {
            new FaixaFreteItemRequest { LimiteInferiorKg = 0, LimiteSuperiorKg = 10, Valor = 1 },
            new FaixaFreteItemRequest { LimiteInferiorKg = 5, LimiteSuperiorKg = 15, Valor = 2 }
        };

        var putResponse = await client.PutAsJsonAsync(
            $"/api/v1/tabelas-frete-cliente/{incidencia.Id}/faixas",
            payload);

        Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
    }

    [Fact]
    public async Task DeveRetornar404QuandoIncidenciaNaoExistir()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");

        var response = await client.GetAsync($"/api/v1/tabelas-frete-cliente/{Guid.NewGuid()}/faixas");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeveIsolarFaixasPorEmbarcador()
    {
        var embarcadorA = Guid.NewGuid();
        var embarcadorB = Guid.NewGuid();

        using var clientA = CreateClient(embarcadorA, "Admin,Operador");
        using var clientB = CreateClient(embarcadorB, "Admin,Operador");

        var tabela = await CriarTabelaFreteAsync(clientA, "ISO");
        var incidencia = await CriarIncidenciaAsync(clientA, tabela.Id);

        var response = await clientB.GetAsync($"/api/v1/tabelas-frete-cliente/{incidencia.Id}/faixas");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static async Task<TabelaFreteResponse> CriarTabelaFreteAsync(HttpClient client, string codigo)
    {
        var response = await client.PostAsJsonAsync("/api/v1/tabelas-frete", new CriarTabelaFreteRequest
        {
            Codigo = codigo,
            Descricao = $"Tabela {codigo}"
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TabelaFreteResponse>())!;
    }

    private static async Task<TabelaFreteClienteResponse> CriarIncidenciaAsync(HttpClient client, Guid tabelaFreteId)
    {
        var response = await client.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabelaFreteId,
            LocalidadeOrigemId = Guid.NewGuid(),
            LocalidadeDestinoId = Guid.NewGuid(),
            VigenciaInicio = new DateOnly(2026, 4, 17),
            VigenciaFim = null
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TabelaFreteClienteResponse>())!;
    }

    private HttpClient CreateClient(Guid embarcadorId, string roles)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.TenantHeader, embarcadorId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, roles);
        client.DefaultRequestHeaders.Add("X-Tenant-Id", embarcadorId.ToString());
        return client;
    }
}
