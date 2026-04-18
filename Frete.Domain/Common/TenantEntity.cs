namespace Frete.Domain.Common;

public abstract class TenantEntity
{
    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid embarcadorId)
    {
        if (embarcadorId == Guid.Empty)
        {
            throw new ArgumentException("Embarcador inválido.", nameof(embarcadorId));
        }

        Id = Guid.NewGuid();
        EmbarcadorId = embarcadorId;
        Ativo = true;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; protected set; }

    public Guid EmbarcadorId { get; protected set; }

    public bool Ativo { get; protected set; }

    public DateTime CreatedAtUtc { get; protected set; }

    public DateTime UpdatedAtUtc { get; protected set; }

    public void Ativar()
    {
        Ativo = true;
        Touch();
    }

    public void Desativar()
    {
        Ativo = false;
        Touch();
    }

    protected void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
