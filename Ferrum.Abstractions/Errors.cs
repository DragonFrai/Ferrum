using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ferrum;


public interface IError
{
    string? Message { get; }
    IError? InnerError { get; }
}

public interface ITracedError : IError
{
    string? StackTrace { get; }
    StackTrace? LocalStackTrace { get; }
}

public interface IAggregateError : IError
{
    bool IsAggregate { get; }
    IEnumerable<IError> InnerErrors { get; }
}
