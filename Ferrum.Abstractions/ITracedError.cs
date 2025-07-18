using System.Diagnostics;

namespace Ferrum;


public interface ITracedError : IError
{
    string? StackTrace { get; }
    // TODO?: Revive `StackTrace? LocalStackTrace` property
}
