namespace Core.Domain;

public abstract class Entity<T>
{
    public T Id { get; set; } = default!;
}