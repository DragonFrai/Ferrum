namespace Ferrum.Errors;


public class ObjectError<T>(T error, Func<T, string>? toString = null) : BaseError
{
    public override string Message => toString is null ? Internal.ToStringOrDefault(error) : toString(error);
    public override IError? InnerError => null;
}
