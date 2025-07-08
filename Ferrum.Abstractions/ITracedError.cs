using System.Diagnostics;

namespace Ferrum;


public interface ITracedError : IError
{
    string? StackTrace { get; }
    StackTrace? LocalStackTrace { get; }
}
