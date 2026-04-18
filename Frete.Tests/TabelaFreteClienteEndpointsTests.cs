using System.Net;
using System.Net.Http.Json;
using Frete.Application.DTOs;

namespace Frete.Tests;

public sealed class TabelaFreteClienteEndpointsTests : IClassFixture<FreteApiFactory>
{
    private readonly FreteApiFactory _factory;

    public TabelaFreteClienteEndpointsTests(FreteApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveCriarAtualizarFiltrarEAlternarStatusDaTabelaFreteCliente()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");

        var tabelaA = await CriarTabelaFreteAsync(client, "A");
        var tabelaB = await CriarTabelaFreteAsync(client, "B");

        var createResponse = await client.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabelaA.Id,
            LocalidadeOrigemId = Guid.NewGuid(),
            LocalidadeDestinoId = Guid.NewGuid(),
            VigenciaInicio = new DateOnly(2026, 4, 17),
            VigenciaFim = new DateOnly(2026, 5, 17)
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TabelaFreteClienteResponse>();
        Assert.NotNull(created);
        Assert.Equal(tabelaA.Id, created!.TabelaFreteId);
        Assert.Equal("A", created.TabelaFreteCodigo);

        await client.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabelaB.Id,
            LocalidadeOrigemId = Guid.NewGuid(),
            LocalidadeDestinoId = Guid.NewGuid(),
            VigenciaInicio = new DateOnly(2026, 4, 18),
            VigenciaFim = null
        });

        var filtered = await client.GetFromJsonAsync<List<TabelaFreteClienteResponse>>($"/api/v1/tabelas-frete-cliente?tabelaFreteId={tabelaA.Id}");
        Assert.NotNull(filtered);
        Assert.Single(filtered!);
        Assert.Equal(tabelaA.Id, filtered![0].TabelaFreteId);

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/tabelas-frete-cliente/{created.Id}", new AtualizarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabelaB.Id,
            LocalidadeOrigemId = created.LocalidadeOrigemId,
            LocalidadeDestinoId = created.LocalidadeDestinoId,
            VigenciaInicio = created.VigenciaInicio,
            VigenciaFim = created.VigenciaFim
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TabelaFreteClienteResponse>();
        Assert.NotNull(updated);
        Assert.Equal(tabelaB.Id, updated!.TabelaFreteId);

        var deactivateResponse = await client.PatchAsync($"/api/v1/tabelas-frete-cliente/{created.Id}/desativar", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);
        var deactivated = await deactivateResponse.Content.ReadFromJsonAsync<TabelaFreteClienteResponse>();
        Assert.NotNull(deactivated);
        Assert.False(deactivated!.Ativo);

        var activateResponse = await client.PatchAsync($"/api/v1/tabelas-frete-cliente/{created.Id}/ativar", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);
        var activated = await activateResponse.Content.ReadFromJsonAsync<TabelaFreteClienteResponse>();
        Assert.NotNull(activated);
        Assert.True(activated!.Ativo);
    }

    [Fact]
    public async Task DeveRejeitarVigenciaFinalMenorQueInicial()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");
        var tabela = await CriarTabelaFreteAsync(client, "C");

        var response = await client.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabela.Id,
            LocalidadeOrigemId = Guid.NewGuid(),
            LocalidadeDestinoId = Guid.NewGuid(),
            VigenciaInicio = new DateOnly(2026, 4, 20),
            VigenciaFim = new DateOnly(2026, 4, 19)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeveIsolarTabelaFreteClientePorEmbarcador()
    {
        var embarcadorA = Guid.NewGuid();
        var embarcadorB = Guid.NewGuid();

        using var clientA = CreateClient(embarcadorA, "Admin,Operador");
        using var clientB = CreateClient(embarcadorB, "Admin,Operador");

        var tabelaA = await CriarTabelaFreteAsync(clientA, "A");
        var tabelaB = await CriarTabelaFreteAsync(clientB, "B");

        await clientA.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabelaA.Id,
            LocalidadeOrigemId = Guid.NewGuid(),
            LocalidadeDestinoId = Guid.NewGuid(),
            VigenciaInicio = new DateOnly(2026, 4, 17),
            VigenciaFim = null
        });

        await clientB.PostAsJsonAsync("/api/v1/tabelas-frete-cliente", new CriarTabelaFreteClienteRequest
        {
            TabelaFreteId = tabelaB.Id,
            LocalidadeOrigemId = Guid.NewGuid(),
            LocalidadeDestinoId = Guid.NewGuid(),
            VigenciaInicio = new DateOnly(2026, 4, 17),
            VigenciaFim = null
        });

        var listA = await clientA.GetFromJsonAsync<List<TabelaFreteClienteResponse>>("/api/v1/tabelas-frete-cliente");
        var listB = await clientB.GetFromJsonAsync<List<TabelaFreteClienteResponse>>("/api/v1/tabelas-frete-cliente");

        Assert.Single(listA!);
        Assert.Single(listB!);
        Assert.Equal(tabelaA.Id, listA![0].TabelaFreteId);
        Assert.Equal(tabelaB.Id, listB![0].TabelaFreteId);
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
