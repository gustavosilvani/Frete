using Frete.Domain.Entities;

namespace Frete.Tests;

public class TabelaFreteClienteEntityTests
{
    [Fact]
    public void Construtor_TabelaFreteVazia_DeveLancar() =>
        Assert.Throws<ArgumentException>(() => new TabelaFreteCliente(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null));

    [Fact]
    public void Construtor_OrigemVazia_DeveLancar() =>
        Assert.Throws<ArgumentException>(() => new TabelaFreteCliente(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), new DateOnly(2026, 1, 1), null));

    [Fact]
    public void Construtor_DestinoVazio_DeveLancar() =>
        Assert.Throws<ArgumentException>(() => new TabelaFreteCliente(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, new DateOnly(2026, 1, 1), null));

    [Fact]
    public void Construtor_VigenciaFimAntesInicio_DeveLancar() =>
        Assert.Throws<ArgumentException>(() => new TabelaFreteCliente(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 6, 1), new DateOnly(2026, 1, 1)));

    [Fact]
    public void Construtor_ValorMinimoNegativo_DeveLancar() =>
        Assert.Throws<ArgumentException>(() => new TabelaFreteCliente(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null, valorMinimo: -1m));

    [Fact]
    public void Atualizar_DeveMudarPropriedades()
    {
        var t = new TabelaFreteCliente(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1), null);
        var novaTabela = Guid.NewGuid();
        t.Atualizar(novaTabela, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 2, 1), new DateOnly(2026, 12, 31), 50m);
        Assert.Equal(novaTabela, t.TabelaFreteId);
        Assert.Equal(50m, t.ValorMinimo);
    }
}
