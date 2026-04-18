using Frete.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frete.Infrastructure.Persistence.Configurations;

public sealed class FaixaFreteConfiguration : IEntityTypeConfiguration<FaixaFrete>
{
    public void Configure(EntityTypeBuilder<FaixaFrete> builder)
    {
        builder.ToTable("faixas_frete");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).ValueGeneratedNever();
        builder.Property(item => item.TabelaFreteClienteId).IsRequired();
        builder.Property(item => item.LimiteInferiorKg).HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.LimiteSuperiorKg).HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.Valor).HasPrecision(18, 2).IsRequired();

        builder.HasOne<TabelaFreteCliente>()
            .WithMany()
            .HasForeignKey(item => item.TabelaFreteClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(item => item.TabelaFreteClienteId);
    }
}
