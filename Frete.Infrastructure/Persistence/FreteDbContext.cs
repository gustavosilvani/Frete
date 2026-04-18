using Frete.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Frete.Infrastructure.Persistence;

public sealed class FreteDbContext : DbContext
{
    public FreteDbContext(DbContextOptions<FreteDbContext> options)
        : base(options)
    {
    }

    public DbSet<TabelaFrete> TabelasFrete => Set<TabelaFrete>();

    public DbSet<TabelaFreteCliente> TabelasFreteCliente => Set<TabelaFreteCliente>();

    public DbSet<FaixaFrete> FaixasFrete => Set<FaixaFrete>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FreteDbContext).Assembly);
    }
}
