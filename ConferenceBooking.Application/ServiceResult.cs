using System;
using System.Collections.Generic;
using System.Text;

namespace ConferenceBooking.Application
{
    //class for delivering a result from operation in service back to calling method. 
    public class ServiceResult
    {
        public bool Success { get; set; } //Was operation successful?
        public string Message { get; set; } = ""; //Message displaying what went wrong to user
        public Dictionary<string, string> Errors { get; set; } = []; //Dictionary with erors, a key to single out issues and handle independently

        public static ServiceResult Ok(string message = "") //method for delivering success message, because operation/validation was successful
            => new() { Success = true, Message = message };

        public static ServiceResult Fail(string message, Dictionary <string, string>? errors = null) //method for delivering failed operation/validation message
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    //same as the one before but can also return data such as an object.
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
        public static ServiceResult<T> Ok(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public new static ServiceResult<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
