# IAggregateError Design


## Message not like in AggregateException

Aggregate errors should not include inner errors messages in their message, as AggregateException does.
Let formatters and other display tools do their job.

The approach with a message like AggregateException certainly signals aggregation quite clearly.
But it is simply not consistent.
AggregateException can be created with one inner exception in the constructor,
but the message will be different from the same constructor with a regular Exception.
And the mapping tools cannot compensate for this behavior without questionable assumptions.


## Nullable InnerErrors

This is useful if the error can be dynamically aggregated or not,
and it is challenging to make two types with different interface implementations
(for example, a json serializable error that can represent inner errors both as an object and as an array).
It also avoids allocating an array of one element if nothing was actually aggregated,
although this must be controlled explicitly and this plays against GetInnerErrors
(because it is forced to allocate an array of one element).


## Super-error aggregator or error-list directly?

IAggregateError is an error with a list of inner errors, not an error that is a list of errors directly.
This has two motivations:
- If the code returns a list of errors,
  then the calling code can rarely use only one of them while maintaining logical correctness.
- The error-list must mimic a single error to cast to IError.

The error-list can mimic:
1. By combining all messages into a common string and all inner errors into one big list.
2. By proxying the implementation of IError itself to one of the inner errors of itself as an aggregated error.

The problem (1) is the ambiguity of such a union.
The locality of messages is lost and can only be restored by type analysis.
The computational complexity of the union is high.

The problem (2) is that if the error was upcast to IError (which is one of the goals of the library),
then one error from the list cannot signal aggregation.

If we aggregate the list of errors into a super-error,
its message can indicate the scope of the aggregation.
