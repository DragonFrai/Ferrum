namespace Ferrum.ExceptionInterop;

public static class ExceptionInteropExtensions
{
    public static IError ToError(this Exception ex)
    {
        return ExceptionErrorImpl.Create(ex);
    }

    // public static IError? GetError(this Exception ex)
    // {
    //     if (ex is IErrorException errorException)
    //     {
    //         return errorException.Error;
    //     }
    //     return null;
    // }

    public static Exception ToException(this IError error)
    {
        return ErrorExceptionImpl.Create(error);
    }

    // public static Exception? GetException(this IError error)
    // {
    //     if (error is IExceptionError exceptionError)
    //     {
    //         return exceptionError.Exception;
    //     }
    //     return null;
    // }

    // public static void Throw(this IError error)
    // {
    //     throw error.ToException();
    // }
}
