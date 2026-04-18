namespace Frete.Domain.Entities;

public sealed class FaixaFrete
{
    private FaixaFrete()
    {
    }

    public FaixaFrete(Guid tabelaFreteClienteId, decimal limiteInferiorKg, decimal limiteSuperiorKg, decimal valor)
    {
        Id = Guid.NewGuid();
        TabelaFreteClienteId = tabelaFreteClienteId;
        LimiteInferiorKg = limiteInferiorKg;
        LimiteSuperiorKg = limiteSuperiorKg;
        Valor = valor;
    }

    public Guid Id { get; private set; }

    public Guid TabelaFreteClienteId { get; private set; }

    public decimal LimiteInferiorKg { get; private set; }

    public decimal LimiteSuperiorKg { get; private set; }

    public decimal Valor { get; private set; }
}
