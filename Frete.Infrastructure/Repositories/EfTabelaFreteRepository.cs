using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frete.Infrastructure.Repositories;

public sealed class EfTabelaFreteRepository : ITabelaFreteRepository
{
    private readonly FreteDbContext _dbContext;

    public EfTabelaFreteRepository(FreteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<TabelaFrete>> ListarPorEmbarcadorAsync(Guid embarcadorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TabelasFrete
            .AsNoTracking()
            .Where(item => item.EmbarcadorId == embarcadorId)
            .OrderBy(item => item.Codigo)
            .ToListAsync(cancellationToken);
    }

    public Task<TabelaFrete?> ObterPorIdAsync(Guid embarcadorId, Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.TabelasFrete
            .FirstOrDefaultAsync(item => item.EmbarcadorId == embarcadorId && item.Id == id, cancellationToken);
    }

    public Task<bool> CodigoExisteAsync(Guid embarcadorId, string codigo, Guid? ignorarId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = codigo.Trim().ToLowerInvariant();

        var query = _dbContext.TabelasFrete
            .AsNoTracking()
            .Where(item => item.EmbarcadorId == embarcadorId && item.Codigo.ToLower() == normalizedCode);

        if (ignorarId.HasValue)
        {
            query = query.Where(item => item.Id != ignorarId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AdicionarAsync(TabelaFrete entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.TabelasFrete.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(TabelaFrete entity, CancellationToken cancellationToken = default)
    {
        _dbContext.TabelasFrete.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
