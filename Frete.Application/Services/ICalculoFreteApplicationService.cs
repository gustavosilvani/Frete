using Frete.Application.DTOs;

namespace Frete.Application.Services;

public interface ICalculoFreteApplicationService
{
    Task<CalcularFreteResponse> CalcularAsync(CalcularFreteRequest request, CancellationToken cancellationToken = default);
}
