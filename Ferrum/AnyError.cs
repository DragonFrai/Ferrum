using Ferrum.Errors;
using Ferrum.ExceptionInterop;

namespace Ferrum;

public static class AnyError
{
    /// <summary>
    /// Convert some value to error representation.
    /// </summary>
    /// <param name="any"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IError Create<T>(T any)
    {
        return any switch
        {
            null => throw new ArgumentNullException(nameof(any)),
            IError err => err,
            Exception ex => ex.ToError(),
            string str => new MessageError(str),
            _ => new ValueError<T>(any)
        };
    }

    // public static IError Create(object any)
    // {
    //     return Create<object>(any);
    // }
}
