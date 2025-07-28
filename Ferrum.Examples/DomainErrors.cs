using System.Diagnostics;

namespace Ferrum.Examples;

public static class EnumWithContextIoErrorExample
{
    public enum IoErrorKind
    {
        FileNotFound,
        PermissionDenied,
    }

    public class IoError(IoErrorKind kind, string path) : IError
    {
        public IoErrorKind Kind { get; } = kind;
        public string Path { get; } = path;

        public string Message
        {
            get
            {
                return Kind switch
                {
                    IoErrorKind.FileNotFound => $"File '{Path}' not found",
                    IoErrorKind.PermissionDenied => $"File '{Path}' can't be opened",
                    _ => throw new UnreachableException()
                };
            }
        }

        public IError? InnerError => null;
    }
}


public static class EnumOnlyIoErrorExample
{
    public enum IoErrorKind
    {
        FileNotFound,
        PermissionDenied,
    }

    public class IoError(IoErrorKind kind) : IError
    {
        public IoErrorKind Kind { get; } = kind;

        public string Message
        {
            get
            {
                return Kind switch
                {
                    IoErrorKind.FileNotFound => $"File not found",
                    IoErrorKind.PermissionDenied => $"File can't be opened",
                    _ => throw new UnreachableException()
                };
            }
        }

        public IError? InnerError => null;
    }
}

public static class InheritanceIoErrorExample
{
    public abstract class IoError : IError
    {
        public abstract string Message { get; }
        public abstract IError? InnerError { get; }
    }

    public class FileNotFoundIoError() : IoError
    {
        public override string Message => "File not found";
        public override IError? InnerError => null;
    }

    public class PermissionDeniedIoError() : IoError
    {
        public override string Message => "Permission denied";
        public override IError? InnerError => null;
    }
}
