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
        DateOnly? vigenciaFim)
        : base(embarcadorId)
    {
        Atualizar(tabelaFreteId, localidadeOrigemId, localidadeDestinoId, vigenciaInicio, vigenciaFim);
    }

    public Guid TabelaFreteId { get; private set; }

    public Guid LocalidadeOrigemId { get; private set; }

    public Guid LocalidadeDestinoId { get; private set; }

    public DateOnly VigenciaInicio { get; private set; }

    public DateOnly? VigenciaFim { get; private set; }

    public void Atualizar(
        Guid tabelaFreteId,
        Guid localidadeOrigemId,
        Guid localidadeDestinoId,
        DateOnly vigenciaInicio,
        DateOnly? vigenciaFim)
    {
        if (tabelaFreteId == Guid.Empty)
        {
            throw new ArgumentException("Tabela de frete é obrigatória.", nameof(tabelaFreteId));
        }

        if (localidadeOrigemId == Guid.Empty)
        {
            throw new ArgumentException("Localidade de origem é obrigatória.", nameof(localidadeOrigemId));
        }

        if (localidadeDestinoId == Guid.Empty)
        {
            throw new ArgumentException("Localidade de destino é obrigatória.", nameof(localidadeDestinoId));
        }

        if (vigenciaFim.HasValue && vigenciaFim.Value < vigenciaInicio)
        {
            throw new ArgumentException("Vigência final deve ser maior ou igual à vigência inicial.", nameof(vigenciaFim));
        }

        TabelaFreteId = tabelaFreteId;
        LocalidadeOrigemId = localidadeOrigemId;
        LocalidadeDestinoId = localidadeDestinoId;
        VigenciaInicio = vigenciaInicio;
        VigenciaFim = vigenciaFim;
        Touch();
    }
}
