using Frete.Application.DTOs;

namespace Frete.Application.Services;

public interface ITabelaFreteApplicationService
{
    Task<IReadOnlyCollection<TabelaFreteResponse>> ListarAsync(CancellationToken cancellationToken = default);

    Task<TabelaFreteResponse> ObterAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TabelaFreteResponse> CriarAsync(CriarTabelaFreteRequest request, CancellationToken cancellationToken = default);

    Task<TabelaFreteResponse> AtualizarAsync(Guid id, AtualizarTabelaFreteRequest request, CancellationToken cancellationToken = default);

    Task<TabelaFreteResponse> AtivarAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TabelaFreteResponse> DesativarAsync(Guid id, CancellationToken cancellationToken = default);
}
