using Frete.Application.DTOs;
using Frete.Application.Faixas;
using Xunit;

namespace Frete.Tests;

public class FaixaFreteValidatorTests
{
    [Fact]
    public void Validar_QuandoExcedeMaximo_DeveLancar()
    {
        var faixas = new List<FaixaFreteItemRequest>();
        for (var i = 0; i < FaixaFreteValidator.MaxFaixasPorIncidencia + 1; i++)
            faixas.Add(new FaixaFreteItemRequest { LimiteInferiorKg = i * 10, LimiteSuperiorKg = (i * 10) + 9, Valor = 1m });

        Assert.Throws<ArgumentException>(() => FaixaFreteValidator.ValidarSubstituicao(faixas));
    }

    [Fact]
    public void Validar_LimiteNegativo_DeveLancar()
    {
        var faixas = new List<FaixaFreteItemRequest>
        {
            new() { LimiteInferiorKg = -1m, LimiteSuperiorKg = 100m, Valor = 1m }
        };
        Assert.Throws<ArgumentException>(() => FaixaFreteValidator.ValidarSubstituicao(faixas));
    }

    [Fact]
    public void Validar_LimiteInferiorMaiorQueSuperior_DeveLancar()
    {
        var faixas = new List<FaixaFreteItemRequest>
        {
            new() { LimiteInferiorKg = 100m, LimiteSuperiorKg = 50m, Valor = 1m }
        };
        Assert.Throws<ArgumentException>(() => FaixaFreteValidator.ValidarSubstituicao(faixas));
    }

    [Fact]
    public void Validar_ValorNegativo_DeveLancar()
    {
        var faixas = new List<FaixaFreteItemRequest>
        {
            new() { LimiteInferiorKg = 0m, LimiteSuperiorKg = 100m, Valor = -1m }
        };
        Assert.Throws<ArgumentException>(() => FaixaFreteValidator.ValidarSubstituicao(faixas));
    }

    [Fact]
    public void Validar_FaixasSobrepostas_DeveLancar()
    {
        var faixas = new List<FaixaFreteItemRequest>
        {
            new() { LimiteInferiorKg = 0m, LimiteSuperiorKg = 100m, Valor = 1m },
            new() { LimiteInferiorKg = 50m, LimiteSuperiorKg = 200m, Valor = 2m }
        };
        Assert.Throws<ArgumentException>(() => FaixaFreteValidator.ValidarSubstituicao(faixas));
    }

    [Fact]
    public void Validar_FaixaUnica_DevePassar()
    {
        var faixas = new List<FaixaFreteItemRequest>
        {
            new() { LimiteInferiorKg = 0m, LimiteSuperiorKg = 100m, Valor = 1m }
        };
        FaixaFreteValidator.ValidarSubstituicao(faixas);
    }

    [Fact]
    public void Validar_FaixasNaoSobrepostas_DevePassar()
    {
        var faixas = new List<FaixaFreteItemRequest>
        {
            new() { LimiteInferiorKg = 0m, LimiteSuperiorKg = 100m, Valor = 1m },
            new() { LimiteInferiorKg = 101m, LimiteSuperiorKg = 200m, Valor = 2m }
        };
        FaixaFreteValidator.ValidarSubstituicao(faixas);
    }

    [Fact]
    public void Validar_ListaVazia_DevePassar()
    {
        FaixaFreteValidator.ValidarSubstituicao(new List<FaixaFreteItemRequest>());
    }
}
