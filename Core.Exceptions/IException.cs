namespace Core.Exceptions;

public interface IException
{
    public int StatusCode { get; }
    public string OutsideMessage { get; }
}