namespace Ferrum;


public interface IAggregateError : IError
{
    bool IsAggregate { get; }
    IEnumerable<IError> InnerErrors { get; }
}
