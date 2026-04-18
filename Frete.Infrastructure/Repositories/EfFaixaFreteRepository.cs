using Frete.Application.Common;
using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frete.Infrastructure.Repositories;

public sealed class EfFaixaFreteRepository : IFaixaFreteRepository
{
    private const string IncidenciaNaoEncontrada = "Tabela de frete cliente não encontrada.";

    private readonly FreteDbContext _dbContext;

    public EfFaixaFreteRepository(FreteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<FaixaFrete>> ListarPorTabelaFreteClienteAsync(
        Guid embarcadorId,
        Guid tabelaFreteClienteId,
        CancellationToken cancellationToken = default)
    {
        var existe = await _dbContext.TabelasFreteCliente
            .AsNoTracking()
            .AnyAsync(
                item => item.Id == tabelaFreteClienteId && item.EmbarcadorId == embarcadorId,
                cancellationToken);

        if (!existe)
        {
            throw new NotFoundException(IncidenciaNaoEncontrada);
        }

        return await _dbContext.FaixasFrete
            .AsNoTracking()
            .Where(item => item.TabelaFreteClienteId == tabelaFreteClienteId)
            .OrderBy(item => item.LimiteInferiorKg)
            .ThenBy(item => item.LimiteSuperiorKg)
            .ToListAsync(cancellationToken);
    }

    public async Task SubstituirColecaoAsync(
        Guid embarcadorId,
        Guid tabelaFreteClienteId,
        IReadOnlyList<FaixaFrete> faixas,
        CancellationToken cancellationToken = default)
    {
        var incidencia = await _dbContext.TabelasFreteCliente
            .FirstOrDefaultAsync(
                item => item.Id == tabelaFreteClienteId && item.EmbarcadorId == embarcadorId,
                cancellationToken);

        if (incidencia is null)
        {
            throw new NotFoundException(IncidenciaNaoEncontrada);
        }

        var antigas = await _dbContext.FaixasFrete
            .Where(item => item.TabelaFreteClienteId == tabelaFreteClienteId)
            .ToListAsync(cancellationToken);

        if (antigas.Count > 0)
        {
            _dbContext.FaixasFrete.RemoveRange(antigas);
        }

        if (faixas.Count > 0)
        {
            await _dbContext.FaixasFrete.AddRangeAsync(faixas, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
