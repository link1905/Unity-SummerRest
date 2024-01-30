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
        protected override void SetAdaptedContentType(ContentType? contentType)
        {
            if (contentType is null)
            {
                base.SetAdaptedContentType(null);
                return;
            }
            // If empty boundary => set a new boundary
            if (string.IsNullOrEmpty(contentType.Value.Boundary))
            {
                var nonEmptyBoundaryContentType = contentType.Value.With(boundary: RequestComponents.ContentType.Commons.RandomBoundary);
                WebRequest.uploadHandler.contentType = nonEmptyBoundaryContentType.FormedContentType;
            }
            else
                WebRequest.uploadHandler.contentType = contentType.Value.FormedContentType;
        }
    }
}