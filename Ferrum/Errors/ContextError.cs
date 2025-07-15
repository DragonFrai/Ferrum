using System.Diagnostics;
using Ferrum.Formatting;

namespace Ferrum.Errors;


public class ContextError(string? message, IError innerError) : BaseError
{
    public ContextError(IError innerError) : this(null, innerError) { }

    public override string Message { get; } = message ?? Helpers.DefaultErrorMessage<ContextError>();
    public override IError? InnerError { get; } = innerError ?? throw new ArgumentNullException(nameof(innerError));
}
