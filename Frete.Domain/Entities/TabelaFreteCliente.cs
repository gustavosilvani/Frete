using Frete.Domain.Common;

namespace Frete.Domain.Entities;

public sealed class TabelaFreteCliente : TenantEntity
{
    private TabelaFreteCliente()
    {
    }

    public TabelaFreteCliente(
        Guid embarcadorId,
        Guid tabelaFreteId,
        Guid localidadeOrigemId,
        Guid localidadeDestinoId,
        DateOnly vigenciaInicio,
        DateOnly? vigenciaFim,
        decimal? valorMinimo = null)
        : base(embarcadorId)
    {
        Atualizar(tabelaFreteId, localidadeOrigemId, localidadeDestinoId, vigenciaInicio, vigenciaFim, valorMinimo);
    }

    public Guid TabelaFreteId { get; private set; }

    public Guid LocalidadeOrigemId { get; private set; }

    public Guid LocalidadeDestinoId { get; private set; }

    public DateOnly VigenciaInicio { get; private set; }

    public DateOnly? VigenciaFim { get; private set; }

    public decimal? ValorMinimo { get; private set; }

    public void Atualizar(
        Guid tabelaFreteId,
        Guid localidadeOrigemId,
        Guid localidadeDestinoId,
        DateOnly vigenciaInicio,
        DateOnly? vigenciaFim,
        decimal? valorMinimo = null)
    {
        if (tabelaFreteId == Guid.Empty)
        {
            throw new ArgumentException("Tabela de frete e obrigatoria.", nameof(tabelaFreteId));
        }

        if (localidadeOrigemId == Guid.Empty)
        {
            throw new ArgumentException("Localidade de origem e obrigatoria.", nameof(localidadeOrigemId));
        }

        if (localidadeDestinoId == Guid.Empty)
        {
            throw new ArgumentException("Localidade de destino e obrigatoria.", nameof(localidadeDestinoId));
        }

        if (vigenciaFim.HasValue && vigenciaFim.Value < vigenciaInicio)
        {
            throw new ArgumentException("Vigencia final deve ser maior ou igual a vigencia inicial.", nameof(vigenciaFim));
        }

        if (valorMinimo.HasValue && valorMinimo.Value < 0)
        {
            throw new ArgumentException("Valor minimo nao pode ser negativo.", nameof(valorMinimo));
        }

        TabelaFreteId = tabelaFreteId;
        LocalidadeOrigemId = localidadeOrigemId;
        LocalidadeDestinoId = localidadeDestinoId;
        VigenciaInicio = vigenciaInicio;
        VigenciaFim = vigenciaFim;
        ValorMinimo = valorMinimo;
        Touch();
    }
}
