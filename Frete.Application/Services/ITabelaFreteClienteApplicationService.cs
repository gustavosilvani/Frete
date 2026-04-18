using Frete.Application.DTOs;

namespace Frete.Application.Services;

public interface ITabelaFreteClienteApplicationService
{
    Task<IReadOnlyCollection<TabelaFreteClienteResponse>> ListarAsync(Guid? tabelaFreteId = null, CancellationToken cancellationToken = default);

    Task<TabelaFreteClienteResponse> ObterAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TabelaFreteClienteResponse> CriarAsync(CriarTabelaFreteClienteRequest request, CancellationToken cancellationToken = default);

    Task<TabelaFreteClienteResponse> AtualizarAsync(Guid id, AtualizarTabelaFreteClienteRequest request, CancellationToken cancellationToken = default);

    Task<TabelaFreteClienteResponse> AtivarAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TabelaFreteClienteResponse> DesativarAsync(Guid id, CancellationToken cancellationToken = default);
}
