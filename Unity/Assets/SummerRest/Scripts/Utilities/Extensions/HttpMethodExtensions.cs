using SummerRest.Scripts.Utilities.RequestComponents;

namespace SummerRest.Scripts.Utilities.Extensions
{
    public static class HttpMethodExtensions
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "Delete";
        public const string Patch = "Patch";
        public const string Head = "HEAD";
        public const string Options = "OPTIONS";
        public const string Trace = "TRACE";
        public const string Connect = "CONNECT";


        // Better performance than Enum.Parse
        public static HttpMethod UnityHttpMethod(string val)
        {
            return val switch
            {
                Get => HttpMethod.Get,
                Post => HttpMethod.Post,
                Put => HttpMethod.Put,
                Delete => HttpMethod.Delete,
                Patch => HttpMethod.Patch,
                Head => HttpMethod.Head,
                Options => HttpMethod.Options,
                Trace => HttpMethod.Trace,
                Connect => HttpMethod.Connect,
                _ => HttpMethod.Get
            };
        }

        public static string ToUnityHttpMethod(this HttpMethod val)
        {
            return val switch
            {
                HttpMethod.Get => Get,
                HttpMethod.Post => Post,
                HttpMethod.Put => Put,
                HttpMethod.Delete => Delete,
                HttpMethod.Patch => Patch,
                HttpMethod.Head => Head,
                HttpMethod.Options => Options,
                HttpMethod.Trace => Trace,
                HttpMethod.Connect => Connect,
                _ => Get
            };
        }
    }
}