using Ferrum.Formatting;

namespace Ferrum.Graphviz;

public class GraphvizErrorFormatter : IErrorFormatter
{
    private GraphvizErrorFormatter() {}

    public static readonly GraphvizErrorFormatter Instance = new();

    public string Format(IError error)
    {
        return Helpers.FormatTree(err => err.Message, err => err.GetInnerErrors(), error);
    }

    public string Format(Exception exception)
    {
        static IEnumerable<Exception> GetInners(Exception ex)
        {
            switch (ex)
            {
                case AggregateException aggregateException:
                    return aggregateException.InnerExceptions;
                default:
                    var innerException = ex.InnerException;
                    return innerException is not null ? [innerException] : [];
            }
        }
        return Helpers.FormatTree(ex => ex.Message, GetInners, exception);
    }
}
