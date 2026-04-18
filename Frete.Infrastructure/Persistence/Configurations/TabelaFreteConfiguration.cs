using Frete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frete.Infrastructure.Persistence.Configurations;

public sealed class TabelaFreteConfiguration : IEntityTypeConfiguration<TabelaFrete>
{
    public void Configure(EntityTypeBuilder<TabelaFrete> builder)
    {
        builder.ToTable("tabelas_frete");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).ValueGeneratedNever();
        builder.Property(item => item.EmbarcadorId).IsRequired();
        builder.Property(item => item.Codigo).HasMaxLength(50).IsRequired();
        builder.Property(item => item.Descricao).HasMaxLength(200).IsRequired();
        builder.Property(item => item.Ativo).IsRequired();
        builder.Property(item => item.CreatedAtUtc).HasColumnType("timestamp with time zone");
        builder.Property(item => item.UpdatedAtUtc).HasColumnType("timestamp with time zone");

        builder.HasIndex(item => item.EmbarcadorId);
        builder.HasIndex(item => new { item.EmbarcadorId, item.Codigo }).IsUnique();
    }
}
