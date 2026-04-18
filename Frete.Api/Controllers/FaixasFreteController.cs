using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frete.Api.Controllers;

[ApiController]
[Route("api/v1/tabelas-frete-cliente/{tabelaFreteClienteId:guid}/faixas")]
[Authorize]
public sealed class FaixasFreteController : ControllerBase
{
    private readonly IFaixaFreteApplicationService _service;

    public FaixasFreteController(IFaixaFreteApplicationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FaixaFreteResponse>>> ListarAsync(
        Guid tabelaFreteClienteId,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.ListarAsync(tabelaFreteClienteId, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<IReadOnlyList<FaixaFreteResponse>>> SubstituirAsync(
        Guid tabelaFreteClienteId,
        [FromBody] FaixaFreteItemRequest[] faixas,
        CancellationToken cancellationToken)
    {
        try
        {
            var lista = faixas ?? Array.Empty<FaixaFreteItemRequest>();
            return Ok(await _service.SubstituirAsync(tabelaFreteClienteId, lista, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
