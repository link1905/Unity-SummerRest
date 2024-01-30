using System.Net;

namespace SummerRest.Runtime.Requests
{
    public struct ResponseError
    {
        public string Message { get; }
        public string ErrorBody { get; }
        public HttpStatusCode StatusCode { get;  }
        public ResponseError(string message, string errorBody, HttpStatusCode statusCode)
        {
            Message = message;
            ErrorBody = errorBody;
            StatusCode = statusCode;
        }
    }
}