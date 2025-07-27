using Ferrum.Errors;
using Xunit;

namespace Ferrum.Examples;

public static class Aggregating
{
    [Fact]
    public static void MultipleAggregating()
    {
        IAggregateError aggError = new AggregateError(
            "Aggregate error",
            new MessageError("Inner error 0"),
            new MessageError("Inner error 1"),
            new MessageError("Inner error 2")
        );

        IError[] innerErrors = aggError.InnerErrors!.ToArray();
        Assert.Equal("Aggregate error", aggError.Message);
        Assert.Equal("Inner error 0", innerErrors[0].Message);
        Assert.Equal("Inner error 1", innerErrors[1].Message);
        Assert.Equal("Inner error 2", innerErrors[2].Message);
    }

    [Fact]
    public static void UsingGetInnerErrors()
    {
        IError aggError = new AggregateError(
            "Aggregate error",
            new MessageError("Inner error 0"),
            new MessageError("Inner error 1"),
            new MessageError("Inner error 2")
        );
        IError notAggError = new MessageError("Not aggregate inner").Context("Not aggregate error");
        IError innerEmptyError = new MessageError("Not inner errors");

        Assert.Equal(3, aggError.GetInnerErrors().Count);
        Assert.Equal(1, notAggError.GetInnerErrors().Count);
        Assert.Equal(0, innerEmptyError.GetInnerErrors().Count);
    }
}
