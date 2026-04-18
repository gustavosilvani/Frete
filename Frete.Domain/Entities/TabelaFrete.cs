using Frete.Domain.Common;

namespace Frete.Domain.Entities;

public sealed class TabelaFrete : TenantEntity
{
    private TabelaFrete()
    {
    }

    public TabelaFrete(Guid embarcadorId, string codigo, string descricao)
        : base(embarcadorId)
    {
        Atualizar(codigo, descricao);
    }

    public string Codigo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public void Atualizar(string codigo, string descricao)
    {
        Codigo = NormalizeRequired(codigo, nameof(codigo), 50, "Código");
        Descricao = NormalizeRequired(descricao, nameof(descricao), 200, "Descrição");
        Touch();
    }

    private static string NormalizeRequired(string? value, string paramName, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} é obrigatório.", paramName);
        }

        var normalized = value.Trim();
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} deve ter no máximo {maxLength} caracteres.", paramName);
        }

        return normalized;
    }
}
