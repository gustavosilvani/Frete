using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frete.Api.Controllers;

[ApiController]
[Route("api/v1/frete")]
[Authorize]
public sealed class CalculoFreteController : ControllerBase
{
    private readonly ICalculoFreteApplicationService _service;

    public CalculoFreteController(ICalculoFreteApplicationService service)
    {
        _service = service;
    }

    [HttpPost("calcular")]
    public async Task<ActionResult<CalcularFreteResponse>> CalcularAsync(
        [FromBody] CalcularFreteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.CalcularAsync(request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
