namespace Core.Exceptions;

public class EntityNotFoundException : Exception, IException
{
    public EntityNotFoundException(string message) : base(message)
    {
        StatusCode = 400;
        OutsideMessage = message;
    }

    public int StatusCode { get; }
    public string OutsideMessage { get; }
}