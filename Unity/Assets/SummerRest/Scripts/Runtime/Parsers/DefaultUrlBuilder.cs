using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SummerRest.Runtime.Request;

namespace SummerRest.Runtime.Parsers
{
    public class DefaultUrlBuilder : IUrlBuilder
    {
        public static string BuildRequestParams(StringBuilder queryStringBuilder, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters)
        {
            foreach (var (key, values) in parameters)
            {
                // URL encode the key and value
                string encodedKey = Uri.EscapeDataString(key);
                if (values.Count <= 0)
                    continue;
                foreach (var value in values)
                {
                    var encodedValue = Uri.EscapeDataString(value);
                    // Append to the query string
                    queryStringBuilder.AppendFormat("{0}={1}&", encodedKey, encodedValue);
                }
            }

            // Remove the trailing "&" if there are parameters
            if (queryStringBuilder.Length > 0)
            {
                queryStringBuilder.Length -= 1;
            }

            return queryStringBuilder.ToString();
        }
        public static string BuildUrl(string url, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters is null)
                return url;
            var paramContainer = new RequestParamContainer();
            foreach (var pair in parameters)
                paramContainer.AddParam(pair.Key, pair.Value);
            return StaticBuildUrl(url, paramContainer.ParamMapper);
        }
        public static string StaticBuildUrl(string url, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters)
        {
            var builder = new StringBuilder();
            builder.Append(url).Append("?");
            var queryString = BuildRequestParams(builder, parameters);
            return queryString;
        }
        public string BuildUrl(string url, IEnumerable<KeyValuePair<string, ICollection<string>>> parameters)
        {
            return StaticBuildUrl(url, parameters);
        }
    }
}