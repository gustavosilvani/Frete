using System.Net;
using System.Net.Http.Json;
using Frete.Application.DTOs;

namespace Frete.Tests;

public sealed class TabelaFreteEndpointsTests : IClassFixture<FreteApiFactory>
{
    private readonly FreteApiFactory _factory;

    public TabelaFreteEndpointsTests(FreteApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveCriarListarAtualizarEAlternarStatusDaTabelaFrete()
    {
        using var client = CreateClient(Guid.NewGuid(), "Admin,Operador");

        var createResponse = await client.PostAsJsonAsync("/api/v1/tabelas-frete", new CriarTabelaFreteRequest
        {
            Codigo = "STD",
            Descricao = "Tabela padrão"
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<TabelaFreteResponse>();
        Assert.NotNull(created);
        Assert.Equal("STD", created!.Codigo);
        Assert.True(created.Ativo);

        var list = await client.GetFromJsonAsync<List<TabelaFreteResponse>>("/api/v1/tabelas-frete");
        Assert.NotNull(list);
        Assert.Single(list!);

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/tabelas-frete/{created.Id}", new AtualizarTabelaFreteRequest
        {
            Codigo = "STD-2",
            Descricao = "Tabela atualizada"
        });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TabelaFreteResponse>();
        Assert.NotNull(updated);
        Assert.Equal("STD-2", updated!.Codigo);

        var deactivateResponse = await client.PatchAsync($"/api/v1/tabelas-frete/{created.Id}/desativar", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.OK, deactivateResponse.StatusCode);

        var deactivated = await deactivateResponse.Content.ReadFromJsonAsync<TabelaFreteResponse>();
        Assert.NotNull(deactivated);
        Assert.False(deactivated!.Ativo);

        var activateResponse = await client.PatchAsync($"/api/v1/tabelas-frete/{created.Id}/ativar", JsonContent.Create(new { }));
        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);

        var activated = await activateResponse.Content.ReadFromJsonAsync<TabelaFreteResponse>();
        Assert.NotNull(activated);
        Assert.True(activated!.Ativo);
    }

    [Fact]
    public async Task DeveImpedirCodigoDuplicadoNoMesmoEmbarcador()
    {
        var embarcadorId = Guid.NewGuid();
        using var client = CreateClient(embarcadorId, "Admin,Operador");

        var firstResponse = await client.PostAsJsonAsync("/api/v1/tabelas-frete", new CriarTabelaFreteRequest
        {
            Codigo = "DUP",
            Descricao = "Primeira"
        });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/tabelas-frete", new CriarTabelaFreteRequest
        {
            Codigo = "DUP",
            Descricao = "Segunda"
        });

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task DeveIsolarListagemPorEmbarcador()
    {
        var embarcadorA = Guid.NewGuid();
        var embarcadorB = Guid.NewGuid();

        using var clientA = CreateClient(embarcadorA, "Admin,Operador");
        using var clientB = CreateClient(embarcadorB, "Admin,Operador");

        await clientA.PostAsJsonAsync("/api/v1/tabelas-frete", new CriarTabelaFreteRequest
        {
            Codigo = "A",
            Descricao = "Tabela A"
        });

        await clientB.PostAsJsonAsync("/api/v1/tabelas-frete", new CriarTabelaFreteRequest
        {
            Codigo = "B",
            Descricao = "Tabela B"
        });

        var listA = await clientA.GetFromJsonAsync<List<TabelaFreteResponse>>("/api/v1/tabelas-frete");
        var listB = await clientB.GetFromJsonAsync<List<TabelaFreteResponse>>("/api/v1/tabelas-frete");

        Assert.Single(listA!);
        Assert.Single(listB!);
        Assert.Equal("A", listA![0].Codigo);
        Assert.Equal("B", listB![0].Codigo);
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
