using Ferrum.Errors;
using Ferrum.Formatting;
using Ferrum.Formatting.Formatters;
using Xunit;

namespace Ferrum.Examples;


public static class Formatting
{
    [Fact]
    public static void FormattingWays()
    {
        IError error =
            new MessageError("I/O error").Context("DB unreachable").Context("User not created");
        const string fmtRes = "User not created: DB unreachable: I/O error";

        Assert.Equal(fmtRes, SummaryErrorFormatter.Instance.Format(error));
        Assert.Equal(fmtRes, error.Format(SummaryErrorFormatter.Instance));

        Assert.Equal(fmtRes, ErrorFormatter.ByFormat("S").Format(error));
        Assert.Equal(fmtRes, error.Format(ErrorFormatter.ByFormat("S")));

        Assert.Equal(fmtRes, ErrorFormatter.ByLevel(2).Format(error));
        Assert.Equal(fmtRes, error.Format(ErrorFormatter.ByLevel(2)));

        Assert.Equal(fmtRes, error.FormatS());
        Assert.Equal(fmtRes, $"{error:S}");
    }
}