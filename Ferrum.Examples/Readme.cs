// using Ferrum;
using Ferrum;
using Ferrum.Errors;
using Ferrum.ExceptionInterop;
using Ferrum.Formatting;

namespace Ferrum.Examples;

public static class ReadmeExample
{

    public static IError? ReadFile(string path, out string content)
    {
        try
        {
            var text = File.ReadAllText(path);
            content = text;
            return null;
        }
        catch (Exception e)
        {
            content = string.Empty;
            return e.ToError();
        }
    }

    public static IError? CreateHello(out string hello)
    {
        var readError = ReadFile("name.txt", out var name);
        if (readError is not null)
        {
            hello = string.Empty;
            return new ContextError("Hello not created", readError);
        }

        hello = $"Hello, {name}!";
        return null;
    }

    public static void ItIsMain(string[] args)
    {
        var helloStringError = CreateHello(out var helloString);
        if (helloStringError is not null)
        {
            // Print:
            // I can't say hello to you: Hello not created: Could not find file '.../name.txt'.
            Console.WriteLine($"I can't say hello: {helloStringError.FormatS()}");
        }
        else
        {
            // Print:
            // I say you: Hello, <name>!
            Console.WriteLine($"I say: {helloString}");
        }
    }
}
