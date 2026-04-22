using System.Net;
using System.Net.Http.Json;
using Frete.Application.DTOs;

namespace Frete.Tests;

public sealed class CalculoFreteEndpointsTests : IClassFixture<FreteApiFactory>
{
    private readonly FreteApiFactory _factory;

    public CalculoFreteEndpointsTests(FreteApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveCalcularFreteComPisoQuandoValorBaseForMenorQueValorMinimo()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");

        var tabela = await CriarTabelaFreteAsync(client, "CALC-001");
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabela.Id,
            LocalidadeOrigemId = origemId,
            LocalidadeDestinoId = destinoId,
            VigenciaInicio = new DateOnly(2026, 4, 1),
            ValorMinimo = 150m
        });

        createResponse.EnsureSuccessStatusCode();
        var tabelaCliente = (await createResponse.Content.ReadFromJsonAsync<TabelaFreteClienteResponse>())!;

        var faixaResponse = await client.PutAsJsonAsync($"/api/v1/tabelas-frete-cliente/{tabelaCliente.Id}/faixas", new[]
        {
            new FaixaFreteItemRequest
            {
                LimiteInferiorKg = 0,
                LimiteSuperiorKg = 100,
                Valor = 100m
            }
        });

        faixaResponse.EnsureSuccessStatusCode();

        var calculoResponse = await client.PostAsJsonAsync("/api/v1/frete/calcular", new CalcularFreteRequest
        {
            LocalidadeOrigemId = origemId,
            LocalidadeDestinoId = destinoId,
            DataReferencia = new DateOnly(2026, 4, 17),
            PesoKg = 50m,
            ComponentesAdicionais = new[]
            {
                new CalculoFreteComponenteRequest
                {
                    Descricao = "Pedagio",
                    Valor = 10m
                }
            }
        });

        Assert.Equal(HttpStatusCode.OK, calculoResponse.StatusCode);
        var payload = await calculoResponse.Content.ReadFromJsonAsync<CalcularFreteResponse>();
        Assert.NotNull(payload);
        Assert.Equal(100m, payload!.ValorBase);
        Assert.Equal(10m, payload.ValorComponentes);
        Assert.True(payload.ValorMinimoAplicado);
        Assert.Equal(150m, payload.ValorTotal);
        Assert.Equal(150m, payload.ValorMinimo);
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

    private HttpClient CreateClient(Guid embarcadorId, string roles)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.TenantHeader, embarcadorId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, roles);
        client.DefaultRequestHeaders.Add("X-Tenant-Id", embarcadorId.ToString());
        return client;
    }
}
