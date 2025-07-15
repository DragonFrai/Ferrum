namespace Ferrum.Errors;


public class ValueError<T>(T value, Func<T, string>? toMessage = null) : BaseError
{
    public override string Message =>
        (toMessage is null ? Helpers.ToString(value) : toMessage(value)) ?? Helpers.DefaultErrorMessage<T>();

    public override IError? InnerError => null;

    public T Value => value;
}
