namespace Ferrum.Examples.UserErrors

open Ferrum

module EnumWithContextIoErrorExample =

    [<RequireQualifiedAccess>]
    type IoErrorKind = FileNotFound | PermissionDenied

    type IoError =
        { Kind: IoErrorKind; Path: string }
        interface IError with
            member this.Message =
                match this.Kind with
                | IoErrorKind.FileNotFound -> $"File '{this.Path}' not found"
                | IoErrorKind.PermissionDenied -> $"File '{this.Path}' can't be opened"
            member this.InnerError = null

module EnumOnlyIoErrorExample =

    [<RequireQualifiedAccess>]
    type IoError =
        | FileNotFound
        | PermissionDenied
        interface IError with
            member this.Message =
                match this with
                | IoError.FileNotFound -> "File not found"
                | IoError.PermissionDenied -> "File can't be opened"
            member this.InnerError = null


module HierarchicalExample =

    type SimpleError =
        | SimpleCase
        interface IError with
            member this.Message =
                match this with
                | SimpleCase -> "Some simple error case"
            member this.InnerError =
                null

    type ComplexError =
        | Source of SimpleError
        | SomeError
        interface IError with
            member this.Message =
                match this with
                | Source _ -> "Error caused by simple error source"
                | SomeError -> "Some complex error case"
            member this.InnerError =
                match this with
                | Source simpleError -> simpleError
                | SomeError -> null

