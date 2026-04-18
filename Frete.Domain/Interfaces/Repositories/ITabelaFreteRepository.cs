using Frete.Domain.Entities;

namespace Frete.Domain.Interfaces.Repositories;

public interface ITabelaFreteRepository
{
    Task<IReadOnlyCollection<TabelaFrete>> ListarPorEmbarcadorAsync(Guid embarcadorId, CancellationToken cancellationToken = default);

    Task<TabelaFrete?> ObterPorIdAsync(Guid embarcadorId, Guid id, CancellationToken cancellationToken = default);

    Task<bool> CodigoExisteAsync(Guid embarcadorId, string codigo, Guid? ignorarId = null, CancellationToken cancellationToken = default);

    Task AdicionarAsync(TabelaFrete entity, CancellationToken cancellationToken = default);

    Task AtualizarAsync(TabelaFrete entity, CancellationToken cancellationToken = default);
}
