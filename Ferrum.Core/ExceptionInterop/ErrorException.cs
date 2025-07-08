namespace Ferrum.ExceptionInterop;


// TODO: Handle IAggregateError converting
public class ErrorException : Exception
{
    private readonly IError _error;

    private static Exception? NewNullable(IError? error)
    {
        return error is null ? null : new ErrorException(error);
    }

    public ErrorException(IError error) : base(error.Message, NewNullable(error.InnerError))
    {
        _error = error;
    }

    // TODO: Override stack trace or not? Flag-based resolve behaviour?
    // public override string? StackTrace => _error.GetStackTrace();
}