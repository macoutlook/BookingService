namespace Core.Exceptions;

// TODO: Instead of custom exceptions in further development it will be handled with custom Result class
public interface IException
{
    public int StatusCode { get; }
    public string OutsideMessage { get; }
}