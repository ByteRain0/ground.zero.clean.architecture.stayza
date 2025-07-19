namespace Stayza.Core.Entity;

public abstract class Entity
{
    [Obsolete("Do not use outside of EF Core integration")]
    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}