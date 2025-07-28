using Ferrum.Errors;
using Ferrum.ExceptionInterop;
using Xunit;

namespace Ferrum.Examples.ExceptionInterop;


public static class ErrorToExceptions
{
    [Fact]
    public static void Simple()
    {
        IError error = new MessageError("Some error");
        Exception ex = error.ToException();

        Assert.Equal("Some error", ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public static void WithInner()
    {
        IError error = new ContextError("Final error", new MessageError("Inner error"));
        Exception ex = error.ToException();

        Assert.Equal("Final error", ex.Message);
        Assert.Equal("Inner error", ex.InnerException!.Message);
    }

    [Fact]
    public static void WithAggregation()
    {
        IError error = new AggregateError(
            "Aggregate error",
            new MessageError("Inner error 0"),
            new MessageError("Inner error 1")
        );
        Exception ex = error.ToException();
        Exception[] innerEx = ((AggregateException) ex).InnerExceptions.ToArray();

        Assert.Equal("Aggregate error (Inner error 0) (Inner error 1)", ex.Message);
        Assert.Equal("Inner error 0", innerEx[0].Message);
        Assert.Equal("Inner error 1", innerEx[1].Message);
    }

    [Fact]
    public static void WithTrace()
    {
        IError error = new TracedMessageError("Traced error");
        Assert.NotNull(error.GetStackTrace());
        Exception ex = error.ToException();

        Assert.Equal("Traced error", ex.Message);
        Assert.Null(ex.InnerException);
        Assert.Null(ex.StackTrace); // Error stack trace does not mirror to exception
    }
}
