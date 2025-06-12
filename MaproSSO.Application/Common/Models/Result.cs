using System;
using System.Collections.Generic;
using System.Linq;

namespace MaproSSO.Application.Common.Models
{
    public class Result<T>
    {
        public bool Succeeded { get; private set; }
        public T Data { get; private set; }
        public string[] Errors { get; private set; }
        public string Message { get; private set; }

        internal Result(bool succeeded, T data, string[] errors, string message = null)
        {
            Succeeded = succeeded;
            Data = data;
            Errors = errors ?? Array.Empty<string>();
            Message = message;
        }

        public static Result<T> Success(T data, string message = null)
        {
            return new Result<T>(true, data, null, message);
        }

        public static Result<T> Failure(params string[] errors)
        {
            return new Result<T>(false, default(T), errors);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default(T), new[] { error });
        }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default(T), errors.ToArray());
        }
    }

    public class Result
    {
        public bool Succeeded { get; private set; }
        public string[] Errors { get; private set; }
        public string Message { get; private set; }

        internal Result(bool succeeded, string[] errors, string message = null)
        {
            Succeeded = succeeded;
            Errors = errors ?? Array.Empty<string>();
            Message = message;
        }

        public static Result Success(string message = null)
        {
            return new Result(true, null, message);
        }

        public static Result Failure(params string[] errors)
        {
            return new Result(false, errors);
        }

        public static Result Failure(string error)
        {
            return new Result(false, new[] { error });
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors.ToArray());
        }
    }
}