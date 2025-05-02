namespace Core.Exceptions;

public sealed class EntityNotFoundException(string message) : Exception(message), IException
{
    public int StatusCode { get; } = 400;
    public string OutsideMessage { get; } = message;
}