using Frete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frete.Infrastructure.Persistence.Configurations;

public sealed class TabelaFreteClienteConfiguration : IEntityTypeConfiguration<TabelaFreteCliente>
{
    public void Configure(EntityTypeBuilder<TabelaFreteCliente> builder)
    {
        builder.ToTable("tabelas_frete_cliente");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).ValueGeneratedNever();
        builder.Property(item => item.EmbarcadorId).IsRequired();
        builder.Property(item => item.TabelaFreteId).IsRequired();
        builder.Property(item => item.LocalidadeOrigemId).IsRequired();
        builder.Property(item => item.LocalidadeDestinoId).IsRequired();
        builder.Property(item => item.VigenciaInicio).HasColumnType("date").IsRequired();
        builder.Property(item => item.VigenciaFim).HasColumnType("date");
        builder.Property(item => item.ValorMinimo).HasPrecision(18, 2);
        builder.Property(item => item.Ativo).IsRequired();
        builder.Property(item => item.CreatedAtUtc).HasColumnType("timestamp with time zone");
        builder.Property(item => item.UpdatedAtUtc).HasColumnType("timestamp with time zone");

        builder.HasOne<TabelaFrete>()
            .WithMany()
            .HasForeignKey(item => item.TabelaFreteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.EmbarcadorId);
        builder.HasIndex(item => new { item.EmbarcadorId, item.TabelaFreteId });
        builder.HasIndex(item => new { item.EmbarcadorId, item.LocalidadeOrigemId, item.LocalidadeDestinoId });
    }
}
