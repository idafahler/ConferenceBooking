using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Dictionary<string, string> Errors { get; set; } = [];

        public static ServiceResult Ok(string message = "")
            => new() { Success = true, Message = message };

        public static ServiceResult Fail(string message, Dictionary <string, string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
        public static ServiceResult<T> Ok(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public new static ServiceResult<T> Fail(string message, Dictionary<string, string>? errors = null)
            => new() { Success = false, Message = message };
    }
}
