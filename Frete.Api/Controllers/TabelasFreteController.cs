using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frete.Api.Controllers;

[ApiController]
[Route("api/v1/tabelas-frete")]
[Authorize]
public sealed class TabelasFreteController : ControllerBase
{
    private readonly ITabelaFreteApplicationService _service;

    public TabelasFreteController(ITabelaFreteApplicationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TabelaFreteResponse>>> ListarAsync(CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TabelaFreteResponse>> ObterAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.ObterAsync(id, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<TabelaFreteResponse>> CriarAsync([FromBody] CriarTabelaFreteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.CriarAsync(request, cancellationToken);
            return Created($"/api/v1/tabelas-frete/{response.Id}", response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<TabelaFreteResponse>> AtualizarAsync(Guid id, [FromBody] AtualizarTabelaFreteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.AtualizarAsync(id, request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/ativar")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<TabelaFreteResponse>> AtivarAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.AtivarAsync(id, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/desativar")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<TabelaFreteResponse>> DesativarAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.DesativarAsync(id, cancellationToken));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
