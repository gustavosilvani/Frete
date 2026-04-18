namespace Frete.Application.DTOs;

public sealed class CriarTabelaFreteRequest
{
    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}

public sealed class AtualizarTabelaFreteRequest
{
    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;
}

public sealed class TabelaFreteResponse
{
    public Guid Id { get; set; }

    public Guid EmbarcadorId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public bool Ativo { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
