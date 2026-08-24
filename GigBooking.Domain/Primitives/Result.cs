using System;
using System.Collections.Generic;
using System.Text;

namespace GigBooking.Domain.Primitives;

public class Result
{
    public bool IsSuccess { get;  }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }


    protected Result(bool isSuccess, Error? error)
    {
        if ((isSuccess && error is not null) || (!isSuccess && error is null))
        {
            throw new InvalidOperationException();
        }   
        IsSuccess = isSuccess;
        Error = error;
    }
    public static Result Success() => new Result(true, null);
    public static Result Failure(Error error) => new Result(false, error);
}
public class Result<T>: Result
{
    public T Value { get; }
    protected internal Result(T value, bool isSuccess, Error? error) : base(isSuccess, error)
    {
        Value = value;
    }
    public static Result<T> Success(T value) => new(value, true, null);
    public new static Result<T> Failure(Error error) => new(default!, false, error);
}