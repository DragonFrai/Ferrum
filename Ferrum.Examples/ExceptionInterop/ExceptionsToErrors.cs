using Ferrum.ExceptionInterop;
using Xunit;

namespace Ferrum.Examples.ExceptionInterop;

public static class ExceptionsToErrors
{
    [Fact]
    public static void Simple()
    {
        Exception ex = new Exception("Simple exception");
        IError error = ex.ToError();

        Assert.Equal("Simple exception", error.Message);
        Assert.Null(error.InnerError);
    }

    [Fact]
    public static void WithInner()
    {
        Exception ex = new Exception("Final exception", new Exception("Inner exception"));
        IError error = ex.ToError();

        Assert.Equal("Final exception", error.Message);
        Assert.Equal("Inner exception", error.InnerError!.Message);
    }

    [Fact]
    public static void WithAggregation()
    {
        Exception ex = new AggregateException(
            "Aggregate exception",
            new Exception("Inner exception 0"),
            new Exception("Inner exception 1")
        );
        IError error = ex.ToError();
        IError[] innerErrors = error.GetInnerErrors().ToArray();

        Assert.Equal("Aggregate exception (Inner exception 0) (Inner exception 1)", error.Message);
        Assert.Equal("Inner exception 0", innerErrors[0].Message);
        Assert.Equal("Inner exception 1", innerErrors[1].Message);
    }

    [Fact]
    public static void WithTrace()
    {
        IError error;
        try
        {
            throw new Exception("Traced exception");
        }
        catch (Exception ex)
        {
            error = ex.ToError();
        }

        Assert.Equal("Traced exception", error.Message);
        Assert.Null(error.InnerError);
        Assert.NotNull(error.GetStackTrace());
    }
}

