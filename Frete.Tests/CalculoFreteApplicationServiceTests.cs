using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Application.Services;
using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Domain.Interfaces.Services;
using Moq;

namespace Frete.Tests;

public class CalculoFreteApplicationServiceTests
{
    private readonly Mock<ITabelaFreteClienteRepository> _tabelaClienteMock = new();
    private readonly Mock<ITabelaFreteRepository> _tabelaMock = new();
    private readonly Mock<IFaixaFreteRepository> _faixaMock = new();
    private readonly Mock<ITenantService> _tenantMock = new();

    private CalculoFreteApplicationService Service() =>
        new(_tabelaClienteMock.Object, _tabelaMock.Object, _faixaMock.Object, _tenantMock.Object);

    private static CalcularFreteRequest Req(Guid? origem = null, Guid? destino = null, decimal peso = 100m, IEnumerable<CalculoFreteComponenteRequest>? comps = null) =>
        new()
        {
            LocalidadeOrigemId = origem ?? Guid.NewGuid(),
            LocalidadeDestinoId = destino ?? Guid.NewGuid(),
            PesoKg = peso,
            ComponentesAdicionais = comps?.ToList()
        };

    [Fact]
    public async Task Calcular_OrigemVazia_DeveLancar() =>
        await Assert.ThrowsAsync<ArgumentException>(() => Service().CalcularAsync(Req(origem: Guid.Empty)));

    [Fact]
    public async Task Calcular_DestinoVazio_DeveLancar() =>
        await Assert.ThrowsAsync<ArgumentException>(() => Service().CalcularAsync(Req(destino: Guid.Empty)));

    [Fact]
    public async Task Calcular_PesoZero_DeveLancar() =>
        await Assert.ThrowsAsync<ArgumentException>(() => Service().CalcularAsync(Req(peso: 0m)));

    [Fact]
    public async Task Calcular_EmbarcadorNaoIdentificado_DeveLancar()
    {
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns((Guid?)null);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Service().CalcularAsync(Req()));
    }

    [Fact]
    public async Task Calcular_TabelaClienteNaoEncontrada_DeveLancarNotFound()
    {
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns(Guid.NewGuid());
        _tabelaClienteMock.Setup(r => r.ObterAplicavelAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TabelaFreteCliente?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => Service().CalcularAsync(Req()));
    }

    [Fact]
    public async Task Calcular_TabelaFreteNaoEncontrada_DeveLancarNotFound()
    {
        var embarcador = Guid.NewGuid();
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns(embarcador);
        var tabelaCliente = new TabelaFreteCliente(embarcador, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null);
        _tabelaClienteMock.Setup(r => r.ObterAplicavelAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tabelaCliente);
        _tabelaMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TabelaFrete?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => Service().CalcularAsync(Req()));
    }

    [Fact]
    public async Task Calcular_SemFaixaParaPeso_DeveLancarNotFound()
    {
        var embarcador = Guid.NewGuid();
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns(embarcador);
        var tabelaCliente = new TabelaFreteCliente(embarcador, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null);
        var tabelaFrete = new TabelaFrete(embarcador, "C1", "D1");
        _tabelaClienteMock.Setup(r => r.ObterAplicavelAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaCliente);
        _tabelaMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaFrete);
        _faixaMock.Setup(r => r.ListarPorTabelaFreteClienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FaixaFrete>());

        await Assert.ThrowsAsync<NotFoundException>(() => Service().CalcularAsync(Req(peso: 50m)));
    }

    [Fact]
    public async Task Calcular_ComponenteNegativo_DeveLancar()
    {
        var embarcador = Guid.NewGuid();
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns(embarcador);
        var tabelaCliente = new TabelaFreteCliente(embarcador, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null);
        var tabelaFrete = new TabelaFrete(embarcador, "C1", "D1");
        var faixa = new FaixaFrete(tabelaCliente.Id, 0m, 1000m, 100m);
        _tabelaClienteMock.Setup(r => r.ObterAplicavelAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaCliente);
        _tabelaMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaFrete);
        _faixaMock.Setup(r => r.ListarPorTabelaFreteClienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<FaixaFrete> { faixa });

        await Assert.ThrowsAsync<ArgumentException>(() => Service().CalcularAsync(
            Req(peso: 50m, comps: new[] { new CalculoFreteComponenteRequest { Valor = -10m } })));
    }

    [Fact]
    public async Task Calcular_ComValorMinimo_DeveAplicarPiso()
    {
        var embarcador = Guid.NewGuid();
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns(embarcador);
        var tabelaCliente = new TabelaFreteCliente(embarcador, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null, valorMinimo: 500m);
        var tabelaFrete = new TabelaFrete(embarcador, "C1", "D1");
        var faixa = new FaixaFrete(tabelaCliente.Id, 0m, 1000m, 100m);
        _tabelaClienteMock.Setup(r => r.ObterAplicavelAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaCliente);
        _tabelaMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaFrete);
        _faixaMock.Setup(r => r.ListarPorTabelaFreteClienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<FaixaFrete> { faixa });

        var result = await Service().CalcularAsync(Req(peso: 50m));

        Assert.True(result.ValorMinimoAplicado);
        Assert.Equal(500m, result.ValorTotal);
    }

    [Fact]
    public async Task Calcular_SemValorMinimo_DeveUsarValorBaseMaisComponentes()
    {
        var embarcador = Guid.NewGuid();
        _tenantMock.Setup(t => t.ObterEmbarcadorIdAtual()).Returns(embarcador);
        var tabelaCliente = new TabelaFreteCliente(embarcador, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null);
        var tabelaFrete = new TabelaFrete(embarcador, "C1", "D1");
        var faixa = new FaixaFrete(tabelaCliente.Id, 0m, 1000m, 100m);
        _tabelaClienteMock.Setup(r => r.ObterAplicavelAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaCliente);
        _tabelaMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(tabelaFrete);
        _faixaMock.Setup(r => r.ListarPorTabelaFreteClienteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<FaixaFrete> { faixa });

        var result = await Service().CalcularAsync(Req(peso: 50m,
            comps: new[] { new CalculoFreteComponenteRequest { Valor = 50m } }));

        Assert.False(result.ValorMinimoAplicado);
        Assert.Equal(150m, result.ValorTotal);
        Assert.Equal(50m, result.ValorComponentes);
    }
}
