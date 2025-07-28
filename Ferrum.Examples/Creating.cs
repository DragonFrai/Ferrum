using Ferrum.Errors;

using Xunit;

namespace Ferrum.Examples;


public static class Creating
{
    [Fact]
    public static void CreatingMessage()
    {
        IError error = new MessageError("Message based error");

        Assert.Equal("Message based error", error.Message);
        Assert.Equal(null, error.InnerError);
    }

    [Fact]
    public static void AddingContext()
    {
        IError cause = new MessageError("Main error");
        IError error = new ContextError("Context error", cause);

        Assert.Equal("Context error", error.Message);
        Assert.Equal("Main error", error.InnerError!.Message);
    }

    [Fact]
    public static void CastAnyToError()
    {
        (string, int, int) notIErrorError = ("invalid token", 12, 48);
        IError error = AnyError.OfValue(notIErrorError);

        Assert.Equal("(invalid token, 12, 48)", error.Message);
    }

}
