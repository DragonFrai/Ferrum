namespace Ferrum.Errors;


public class MessageError(string? message = null) : BaseError
{
    public override string Message { get; } = message ?? Helpers.DefaultErrorMessage<MessageError>();

    public override IError? InnerError => null;
}
