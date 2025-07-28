namespace Ferrum;


public interface IAggregateError : IError
{
    /// <summary>
    /// Gets a read-only collection of the IError instances that caused the current error.
    /// </summary>
    /// <returns>
    /// A read-only collection of the IError instances that caused the current error.
    /// </returns>
    /// <remarks>
    /// If not null and not empty, then <see cref="Ferrum.IError.InnerError"/>
    /// must be first element of this collection.
    /// If null, then this error is semantically not aggregated and you must get the inner error
    /// using <see cref="Ferrum.IError.InnerError"/> to avoid allocating a single element collection.
    /// If semantic checks and allocations are not critical,
    /// see <see cref="Ferrum.ErrorExtensions.GetInnerErrors"/> extension.
    /// </remarks>
    public IReadOnlyCollection<IError>? InnerErrors { get; }
}
