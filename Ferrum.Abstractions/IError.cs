namespace Ferrum;


public interface IError
{
    string Message { get; }
    IError? InnerError { get; }
}
