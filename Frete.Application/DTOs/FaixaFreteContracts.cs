namespace Frete.Application.DTOs;

public sealed class FaixaFreteItemRequest
{
    public decimal LimiteInferiorKg { get; set; }

    public decimal LimiteSuperiorKg { get; set; }

    public decimal Valor { get; set; }
}

public sealed class FaixaFreteResponse
{
    public Guid Id { get; set; }

    public Guid TabelaFreteClienteId { get; set; }

    public decimal LimiteInferiorKg { get; set; }

    public decimal LimiteSuperiorKg { get; set; }

    public decimal Valor { get; set; }
}
