using System;

namespace SummerRest.Runtime.Requests
{
    public class ResponseErrorException : Exception
    {
        public ResponseError Error { get; }
        public ResponseErrorException(ResponseError error)
        {
            Error = error;
        }
    }
}