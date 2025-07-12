using System.Diagnostics;

namespace Ferrum.Errors;


public class AggregateError : BaseError, IAggregateError
{
    private readonly string _message;
    private readonly IReadOnlyCollection<IError> _innerErrors;

    private static ArgumentNullException ErrorIsNull(string argName, int index)
    {
        return new ArgumentNullException($"{argName}[{index}]");
    }

    public AggregateError(string message, IReadOnlyList<IError> innerErrors, bool cloneInnerErrors = true)
    {
        ArgumentNullException.ThrowIfNull(innerErrors, nameof(innerErrors));
        var count = innerErrors.Count;
        IReadOnlyCollection<IError> innerErrorsCollection;
        if (cloneInnerErrors)
        {
            var innerErrorsArray = new IError[count];
            for (var i = 0; i < count; i++)
            {
                var error = innerErrors[i];
                innerErrorsArray[i] = error ?? throw ErrorIsNull(nameof(innerErrors), i);
            }
            innerErrorsCollection = innerErrorsArray;
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                var error = innerErrors[i];
                if (error is null)
                {
                    throw ErrorIsNull(nameof(innerErrors), i);
                }
            }
            innerErrorsCollection = innerErrors;
        }

        _innerErrors = innerErrorsCollection;
        _message = message;
    }

    public AggregateError(string message, IEnumerable<IError> innerErrors)
        : this(message, innerErrors.ToArray(), false) { }

    public AggregateError(string message)
        : this(message, [], false) { }

    public AggregateError()
        : this("Aggregate error", [], false) { }

    public override string Message => _message;

    public override IError? InnerError => _innerErrors.FirstOrDefault();

    public bool IsAggregate => true;

    public IEnumerable<IError> InnerErrors => _innerErrors;
}
