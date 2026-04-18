using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frete.Infrastructure.Repositories;

public sealed class EfTabelaFreteClienteRepository : ITabelaFreteClienteRepository
{
    private readonly FreteDbContext _dbContext;

    public EfTabelaFreteClienteRepository(FreteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<TabelaFreteCliente>> ListarPorEmbarcadorAsync(Guid embarcadorId, Guid? tabelaFreteId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TabelasFreteCliente
            .AsNoTracking()
            .Where(item => item.EmbarcadorId == embarcadorId);

        if (tabelaFreteId.HasValue)
        {
            query = query.Where(item => item.TabelaFreteId == tabelaFreteId.Value);
        }

        return await query
            .OrderByDescending(item => item.VigenciaInicio)
            .ThenBy(item => item.LocalidadeOrigemId)
            .ThenBy(item => item.LocalidadeDestinoId)
            .ToListAsync(cancellationToken);
    }

    public Task<TabelaFreteCliente?> ObterPorIdAsync(Guid embarcadorId, Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.TabelasFreteCliente
            .FirstOrDefaultAsync(item => item.EmbarcadorId == embarcadorId && item.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(TabelaFreteCliente entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.TabelasFreteCliente.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(TabelaFreteCliente entity, CancellationToken cancellationToken = default)
    {
        _dbContext.TabelasFreteCliente.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
