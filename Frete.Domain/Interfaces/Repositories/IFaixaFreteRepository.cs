using Frete.Domain.Entities;

namespace Frete.Domain.Interfaces.Repositories;

public interface IFaixaFreteRepository
{
    Task<IReadOnlyList<FaixaFrete>> ListarPorTabelaFreteClienteAsync(
        Guid embarcadorId,
        Guid tabelaFreteClienteId,
        CancellationToken cancellationToken = default);

    Task SubstituirColecaoAsync(
        Guid embarcadorId,
        Guid tabelaFreteClienteId,
        IReadOnlyList<FaixaFrete> faixas,
        CancellationToken cancellationToken = default);
}
