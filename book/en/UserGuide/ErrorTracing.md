# Error Tracing

Errors implementing `ITracedError` can have a stack trace in the `StackTrace` property.
To get a stack trace for any error without explicit type checks, use the `GetStackTrace` extension.

Stack traces are used in `DetailedErrorFormatter` and `DiagnosticErrorFormatter`.
See [Error Formatting](ErrorFormatting.md) for more details.

Most of the errors built in Ferrum have stack trace variants. 
They collect the stack trace when they are created.

| Without stack trace | With stack trace     |
|---------------------|----------------------|
| MessageError        | TracedMessageError   |
| ContextError        | TracedContextError   |
| AggregateError      | TracedAggregateError |

You may want to capture stack traces or not depending on environment variables or build type (release/debug).
`Ferrum` does not currently provide default solutions for these scenarios.
You can declare your own functions for these purposes.

Capturing stack traces takes time, so use this feature carefully.
If the error value returned by a function is intended to be used for branches,
and not just returned to the caller, then capturing stack traces can have an impact on performance.
In such use cases, the recommendations for using errors with stack traces are the same as for thrown exceptions.

<div class="warning">
If you implement an error type, you don't need to worry about your error storing a stack trace.
Domain errors are weakly dependent on a stack trace.
They are usually either handled in code without needing to view the trace, 
or upcasted to IError and wrapped in a context error that may contain a stack trace.
For these reasons, Ferrum's design does not focus on designing errors with stack traces in user space.

But you also shouldn't exclude the possibility of implementing the <code>ITracedError</code> interface
if you think it will be useful in a particular case.
</div>
