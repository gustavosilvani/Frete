using Frete.Domain.Entities;

namespace Frete.Domain.Interfaces.Repositories;

public interface ITabelaFreteClienteRepository
{
    Task<IReadOnlyCollection<TabelaFreteCliente>> ListarPorEmbarcadorAsync(Guid embarcadorId, Guid? tabelaFreteId = null, CancellationToken cancellationToken = default);

    Task<TabelaFreteCliente?> ObterPorIdAsync(Guid embarcadorId, Guid id, CancellationToken cancellationToken = default);

    Task AdicionarAsync(TabelaFreteCliente entity, CancellationToken cancellationToken = default);

    Task AtualizarAsync(TabelaFreteCliente entity, CancellationToken cancellationToken = default);
}
