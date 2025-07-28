namespace Ferrum;


public interface IError
{
    public string Message { get; }
    public IError? InnerError { get; }
}
