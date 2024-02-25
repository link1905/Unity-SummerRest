using System;
using System.Collections.Generic;
using System.Net;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// Contains response data of a web request <br/>
    /// Please remember to wrap this instance inside a using statement or manually call <see cref="IWebResponse{TBody}.Dispose()"/>
    /// </summary>
    /// <typeparam name="TBody"></typeparam>
    public interface IWebResponse<out TBody> : IDisposable
    {
        public object WrappedRequest { get;  }
        public HttpStatusCode StatusCode { get;  }
        public ContentType ContentType { get; }
        public IEnumerable<KeyValuePair<string, string>> Headers { get; }
        public string RawText { get;  }
        public byte[] RawData { get; }
        public TBody Data { get;  }
    }
}