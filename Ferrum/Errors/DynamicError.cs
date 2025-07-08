using System.Diagnostics;

namespace Ferrum.Errors;

/*
public class DynamicError : IError, IAggregateError, ITracedError
{
    private string _message;
    private IError? _innerError;
    private bool _isAggregate;
    private IEnumerable<IError> _innerErrors;
    private string? _stackTrace;
    private StackTrace? _localStackTrace;

    public DynamicError(string message, IError? innerError = null, StackTrace? localStackTrace = null)
    {
        _message = message;
        _innerError = innerError;
        _isAggregate = false;
        _innerErrors = innerError is null ? [] : [innerError];
        _stackTrace = localStackTrace?.ToString();
        _localStackTrace = localStackTrace;
    }

    public DynamicError(string message, IError? innerError = null, string? stackTrace = null)
    {
        _message = message;
        _innerError = innerError;
        _isAggregate = false;
        _innerErrors = innerError is null ? [] : [innerError];
        _stackTrace = stackTrace;
        _localStackTrace = null;
    }

    public DynamicError(string message, IEnumerable<IError> innerErrors, string? stackTrace = null)
    {
        _message = message;
        _innerError = Internal.TryFirst(innerErrors);
        _isAggregate = false;
        _innerErrors = innerErrors;
        _stackTrace = stackTrace;
        _localStackTrace = null;
    }

    public DynamicError(string message, IEnumerable<IError> innerErrors, StackTrace? localStackTrace = null)
    {
        _message = message;
        _innerError = Internal.TryFirst(innerErrors);
        _isAggregate = false;
        _innerErrors = innerErrors;
        _stackTrace = localStackTrace?.ToString();
        _localStackTrace = localStackTrace;
    }

    public string Message => _message;
    public IError? InnerError => _innerError;
    public bool IsAggregate => _isAggregate;
    public IEnumerable<IError> InnerErrors => _innerErrors;
    public string? StackTrace => _stackTrace;
    public StackTrace? LocalStackTrace => _localStackTrace;

}
*/

/*
type DynamicError =
   val private message: string
   val private innerError: IError
   val private innerErrors: IError seq
   val private isAggregate: bool
   val private trace: string
   val private localTrace: StackTrace

   new(message: string) =
       { message = message; innerError = null; innerErrors = Seq.empty; isAggregate = false; trace = null; localTrace = null }

   new(message: string, source: IError) =
       { message = message; innerError = source; innerErrors = Seq.singleton source; isAggregate = false; trace = null; localTrace = null }

   new(message: string, sources: IError seq) =
       { message = message; innerError = Utils.tryFirst sources |> ValueOption.toObj; innerErrors = sources; isAggregate = true; trace = null; localTrace = null }

   new(message: string, trace: string) =
       { message = message; innerError = null; innerErrors = Seq.empty; isAggregate = false; trace = trace; localTrace = null }

   new(message: string, source: IError, trace: string) =
       { message = message; innerError = source; innerErrors = Seq.singleton source; isAggregate = false; trace = trace; localTrace = null }

   new(message: string, sources: IError seq, trace: string) =
       { message = message; innerError = Utils.tryFirst sources |> ValueOption.toObj; innerErrors = sources; isAggregate = true; trace = trace; localTrace = null }

   new(message: string, localTrace: StackTrace) =
       { message = message; innerError = null; innerErrors = Seq.empty; isAggregate = false; trace = localTrace.ToString(); localTrace = localTrace }

   new(message: string, source: IError, localTrace: StackTrace) =
       { message = message; innerError = source; innerErrors = Seq.singleton source; isAggregate = false; trace = localTrace.ToString(); localTrace = localTrace }

   new(message: string, sources: IError seq, localTrace: StackTrace) =
       { message = message; innerError = Utils.tryFirst sources |> ValueOption.toObj; innerErrors = sources; isAggregate = true; trace = localTrace.ToString(); localTrace = localTrace }

   interface IError with
       member this.Message = this.message
       member this.InnerError = this.innerError

   interface IAggregateError with
       member this.IsAggregate = this.isAggregate
       member this.InnerErrors = this.innerErrors

   interface ITracedError with
       member this.StackTrace = this.trace
       member this.LocalStackTrace = this.localTrace

   override this.ToString() =
       ErrorFormatter.Default.Format(this)

   interface IFormattable with
       member this.ToString(format, _formatProvider) =
           (ErrorFormatter.byFormat format).Format(this)

*/