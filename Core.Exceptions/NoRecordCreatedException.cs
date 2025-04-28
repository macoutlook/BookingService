namespace Core.Exceptions;

public class NoRecordCreatedException : Exception, IException
{
    public NoRecordCreatedException(string message) : base(message)
    {
        StatusCode = 400;
        OutsideMessage = message;
    }

    public int StatusCode { get; }
    public string OutsideMessage { get; }
}