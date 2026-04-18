using Frete.Application.DTOs;

namespace Frete.Application.Services;

public interface IFaixaFreteApplicationService
{
    Task<IReadOnlyList<FaixaFreteResponse>> ListarAsync(Guid tabelaFreteClienteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FaixaFreteResponse>> SubstituirAsync(
        Guid tabelaFreteClienteId,
        IReadOnlyList<FaixaFreteItemRequest> faixas,
        CancellationToken cancellationToken = default);
}
