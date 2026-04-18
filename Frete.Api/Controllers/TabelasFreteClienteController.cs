using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frete.Api.Controllers;

[ApiController]
[Route("api/v1/tabelas-frete-cliente")]
[Authorize]
public sealed class TabelasFreteClienteController : ControllerBase
{
    private readonly ITabelaFreteClienteApplicationService _service;

    public TabelasFreteClienteController(ITabelaFreteClienteApplicationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TabelaFreteClienteResponse>>> ListarAsync(
        [FromQuery] Guid? tabelaFreteId,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.ListarAsync(tabelaFreteId, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TabelaFreteClienteResponse>> ObterAsync(Guid id, CancellationToken cancellationToken)
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
    public async Task<ActionResult<TabelaFreteClienteResponse>> CriarAsync(
        [FromBody] CriarTabelaFreteClienteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.CriarAsync(request, cancellationToken);
            return Created($"/api/v1/tabelas-frete-cliente/{response.Id}", response);
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<TabelaFreteClienteResponse>> AtualizarAsync(
        Guid id,
        [FromBody] AtualizarTabelaFreteClienteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.AtualizarAsync(id, request, cancellationToken));
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

    [HttpPatch("{id:guid}/ativar")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<ActionResult<TabelaFreteClienteResponse>> AtivarAsync(Guid id, CancellationToken cancellationToken)
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
    public async Task<ActionResult<TabelaFreteClienteResponse>> DesativarAsync(Guid id, CancellationToken cancellationToken)
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
