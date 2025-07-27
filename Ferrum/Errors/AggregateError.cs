using System.Collections.ObjectModel;

namespace Ferrum.Errors;


public class AggregateError : BaseError, IAggregateError
{
    private readonly string _message;
    private readonly IError[] _innerErrors;
    private IReadOnlyCollection<IError>? _rocView;

    private AggregateError(string? message, IError[] innerErrors, IReadOnlyCollection<IError>? rocView)
    {
        ArgumentNullException.ThrowIfNull(innerErrors, nameof(innerErrors));
        var nullAt = Helpers.CheckNotNullItems(innerErrors);
        if (nullAt != -1)
        {
            throw new ArgumentNullException($"{nameof(innerErrors)}[{nullAt}]");
        }
        _message = message ?? Helpers.DefaultErrorMessage<AggregateError>();
        _innerErrors = innerErrors;
        _rocView = rocView;
    }

    public AggregateError(string? message, params IError[] innerErrors)
        : this(message, Helpers.ArrayCopy(innerErrors), null) { }

    public AggregateError(string? message, IEnumerable<IError> innerErrors)
        : this(message, innerErrors.ToArray(), null) { }

    public AggregateError(string? message)
        : this(message, [], null) { }

    public AggregateError()
        : this(null, [], null) { }

    public override string Message => _message;

    public override IError? InnerError => _innerErrors.FirstOrDefault();

    public IReadOnlyCollection<IError> InnerErrors
    {
        get
        {
            if (_rocView is not null) return _rocView;
            var rocView = _innerErrors.Length != 0
                ? new ReadOnlyCollection<IError>(_innerErrors)
                : ReadOnlyCollection<IError>.Empty;
            return _rocView = rocView;
        }
    }
}
