namespace Ferrum.Formatting.Formatters;

public class MessageErrorFormatter : IErrorFormatter
{
    private MessageErrorFormatter() { }

    public static readonly MessageErrorFormatter Instance = new MessageErrorFormatter();

    public string Format(IError error)
    {
        return error.Message;
    }
}
