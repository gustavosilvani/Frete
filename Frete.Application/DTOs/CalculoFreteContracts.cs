namespace Frete.Application.DTOs;

public sealed class CalcularFreteRequest
{
    public Guid LocalidadeOrigemId { get; set; }

    public Guid LocalidadeDestinoId { get; set; }

    public DateOnly? DataReferencia { get; set; }

    public decimal PesoKg { get; set; }

    public decimal? VolumeM3 { get; set; }

    public int? QuantidadePallets { get; set; }

    public IReadOnlyList<CalculoFreteComponenteRequest>? ComponentesAdicionais { get; set; }
}

public sealed class CalculoFreteComponenteRequest
{
    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }
}

public sealed class CalcularFreteResponse
{
    public Guid TabelaFreteClienteId { get; set; }

    public Guid TabelaFreteId { get; set; }

    public string TabelaFreteCodigo { get; set; } = string.Empty;

    public string TabelaFreteDescricao { get; set; } = string.Empty;

    public DateOnly DataReferencia { get; set; }

    public decimal PesoKg { get; set; }

    public decimal ValorBase { get; set; }

    public decimal ValorComponentes { get; set; }

    public decimal? ValorMinimo { get; set; }

    public bool ValorMinimoAplicado { get; set; }

    public decimal ValorTotal { get; set; }

    public CalculoFreteFaixaResponse FaixaSelecionada { get; set; } = new();
}

public sealed class CalculoFreteFaixaResponse
{
    public Guid Id { get; set; }

    public decimal LimiteInferiorKg { get; set; }

    public decimal LimiteSuperiorKg { get; set; }

    public decimal Valor { get; set; }
}
