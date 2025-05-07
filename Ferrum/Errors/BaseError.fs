namespace Ferrum

open System


[<AbstractClass>]
type BaseError() =

    abstract Message: string
    abstract InnerError: IError voption

    interface IError with
        member this.InnerError = this.InnerError
        member this.Message = this.Message

    interface IFormattable with
        member this.ToString(format, _formatProvider) =
            let formatter = ErrorFormatters.getByFormat format
            formatter.Format(this)

    override this.ToString() =
        ErrorFormatters.General.Format(this)
