using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Parsers
{
    /// <summary>
    /// Instruct on how to read a response's content-type <br/>
    /// Default support is <see cref="DefaultContentTypeParser"/> based on common conventions of HTTP and <see cref="UnityWebRequest"/> <br/>
    /// You can adapt <see cref="IDefaultSupport{TInterface,TDefault}.Current"/> to your preference in runtime
    /// </summary>
    public interface IContentTypeParser : IDefaultSupport<IContentTypeParser, DefaultContentTypeParser>
    {
        /// <summary>
        /// The name of the header used to retrieved content-type string 
        /// </summary>
        string ContentTypeHeaderKey { get; }
        /// <summary>
        /// Get data format of a response from a content-type string
        /// </summary>
        /// <param name="contentTypeHeader"></param>
        /// <returns></returns>
        DataFormat ParseDataFormatFromResponse(string contentTypeHeader);
        /// <summary>
        /// Get content-type of a response from a content-type string
        /// </summary>
        /// <param name="contentTypeHeader"></param>
        /// <returns></returns>
        ContentType ParseContentTypeFromHeader(string contentTypeHeader);
    }
}