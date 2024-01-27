using System.Text;
using SummerRest.Runtime.RequestComponents;
using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adapting a multi-part UnityWebRequest
    /// </summary>
    internal class MultipartFileUnityWebRequestAdaptor<TResponse> : RawUnityWebRequestAdaptor<TResponse>
    {
        public byte[] Data => WebRequest.uploadHandler.data;
        protected override void SetAdaptedContentType(in ContentType contentType)
        {
            if (string.IsNullOrEmpty(contentType.Boundary))
            {
                var nonEmptyBoundaryContentType = contentType.With(boundary: Encoding.UTF8.GetString(UnityWebRequest.GenerateBoundary()));
                WebRequest.uploadHandler.contentType = nonEmptyBoundaryContentType.FormedContentType;
            }
            else
                WebRequest.uploadHandler.contentType = contentType.FormedContentType;
        }
    }
}