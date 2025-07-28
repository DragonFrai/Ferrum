namespace Ferrum;


public interface ITracedError : IError
{
    /// <summary>
    /// true if StackTrace property not null.
    /// </summary>
    public bool HasStackTrace { get; } // => StackTrace is not null;

    public string? StackTrace { get; }

    // TODO?: Revive `StackTrace? LocalStackTrace` property
}
