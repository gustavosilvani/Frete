namespace Frete.Application.DTOs;

public sealed class CriarTabelaFreteClienteRequest
{
    public Guid TabelaFreteId { get; set; }

    public Guid LocalidadeOrigemId { get; set; }

    public Guid LocalidadeDestinoId { get; set; }

    public DateOnly VigenciaInicio { get; set; }

    public DateOnly? VigenciaFim { get; set; }
}

public sealed class AtualizarTabelaFreteClienteRequest
{
    public Guid TabelaFreteId { get; set; }

    public Guid LocalidadeOrigemId { get; set; }

    public Guid LocalidadeDestinoId { get; set; }

    public DateOnly VigenciaInicio { get; set; }

    public DateOnly? VigenciaFim { get; set; }
}

public sealed class TabelaFreteClienteResponse
{
    public Guid Id { get; set; }

    public Guid EmbarcadorId { get; set; }

    public Guid TabelaFreteId { get; set; }

    public string TabelaFreteCodigo { get; set; } = string.Empty;

    public string TabelaFreteDescricao { get; set; } = string.Empty;

    public Guid LocalidadeOrigemId { get; set; }

    public Guid LocalidadeDestinoId { get; set; }

    public DateOnly VigenciaInicio { get; set; }

    public DateOnly? VigenciaFim { get; set; }

    public bool Ativo { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
