using System.Diagnostics;
using System.Globalization;

namespace Ferrum;


internal static class Helpers
{
    public static string DefaultErrorMessage<T>()
    {
        var type = typeof(T);
        return $"Error[{type.Name}]";
    }

    public static string MessageOrDefault<T>(string? message)
    {
        return message ?? DefaultErrorMessage<T>();
    }

    public static string? ToString<T>(T value)
    {
        return value switch
        {
            IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            null => null,
            _ => value.ToString() ?? null
        };
    }

    public static T PassNotNull<T>(T? value, string? argName)
    {
        return value ?? throw new ArgumentNullException(argName);
    }

    public static int CheckNotNullItems<T>(T[] list, bool cloneList, out T[] resultList)
    {
        var count = list.Length;
        if (cloneList)
        {
            var innerErrorsArray = new T[count];
            for (var i = 0; i < count; i++)
            {
                var error = list[i];
                if (error is null)
                {
                    resultList = [];
                    return i;
                }
                innerErrorsArray[i] = error;
            }
            resultList = innerErrorsArray;
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                var error = list[i];
                if (error is null)
                {
                    resultList = [];
                    return i;
                }
            }
            resultList = list;
        }
        return -1;
    }

    public static int CheckNotNullItems<T>(T[] list)
    {
        var count = list.Length;
        for (var i = 0; i < count; i++)
        {
            var error = list[i];
            if (error is null)
            {
                return i;
            }
        }
        return -1;
    }

    public static T[] ArrayCopy<T>(T[] array)
    {
        var copy = new T[array.Length];
        Array.Copy(array, copy, array.Length);
        return copy;
    }

}

/// <summary>
/// Internal util for caching stack trace string
/// </summary>
// Okay... How to test it in the dedicated project?
internal struct StackTraceCell
{
    private readonly StackTrace? _stackTrace;
    private string? _stackTraceString;

    public StackTraceCell(StackTrace? stackTrace, string? stackTraceString)
    {
        _stackTrace = stackTrace;
        _stackTraceString = stackTraceString;
    }

    [StackTraceHidden]
    public StackTraceCell() : this(new StackTrace(0, true), null)
    { }

    public string? GetStackTrace()
    {
        if (_stackTraceString is not null)
        {
            return _stackTraceString;
        }

        // ReSharper disable once InvertIf
        if (_stackTrace is not null)
        {
            var stackTraceString = _stackTrace.ToString();
            _stackTraceString = stackTraceString;
            return stackTraceString;
        }

        return null;
    }

    public StackTrace? GetLocalStackTrace()
    {
        return _stackTrace;
    }

}
