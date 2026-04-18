using Frete.Application.DTOs;

namespace Frete.Application.Faixas;

public static class FaixaFreteValidator
{
    public const int MaxFaixasPorIncidencia = 100;

    public static void ValidarSubstituicao(IReadOnlyList<FaixaFreteItemRequest> faixas)
    {
        if (faixas.Count > MaxFaixasPorIncidencia)
        {
            throw new ArgumentException(
                $"É permitido no máximo {MaxFaixasPorIncidencia} faixas por incidência.",
                nameof(faixas));
        }

        foreach (var item in faixas)
        {
            if (item.LimiteInferiorKg < 0 || item.LimiteSuperiorKg < 0)
            {
                throw new ArgumentException("Limites de peso não podem ser negativos.");
            }

            if (item.LimiteInferiorKg > item.LimiteSuperiorKg)
            {
                throw new ArgumentException("O limite inferior deve ser menor ou igual ao limite superior.");
            }

            if (item.Valor < 0)
            {
                throw new ArgumentException("O valor da faixa não pode ser negativo.");
            }
        }

        if (faixas.Count <= 1)
        {
            return;
        }

        var ordenadas = faixas
            .OrderBy(item => item.LimiteInferiorKg)
            .ThenBy(item => item.LimiteSuperiorKg)
            .ToList();

        for (var i = 1; i < ordenadas.Count; i++)
        {
            if (ordenadas[i].LimiteInferiorKg <= ordenadas[i - 1].LimiteSuperiorKg)
            {
                throw new ArgumentException(
                    "As faixas não podem se sobrepor nem se tocar nos limites (cada quilograma deve pertencer a no máximo uma faixa).");
            }
        }
    }
}
