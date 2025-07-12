using System.Diagnostics;
using Ferrum.Formatting;

namespace Ferrum.Errors;


public class ContextError(string message, IError innerError) : BaseError, IFormattable
{
    public override string Message { get; } = message;
    public override IError? InnerError { get; } = innerError ?? throw new ArgumentNullException(nameof(innerError));
}
