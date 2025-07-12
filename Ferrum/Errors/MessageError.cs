namespace Ferrum.Errors;


public class MessageError(string message) : BaseError
{
    public override string Message => message;
    public override IError? InnerError => null;
}
