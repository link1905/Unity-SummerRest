using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adapting a multi-part UnityWebRequest
    /// </summary>
    internal class MultipartFileUnityWebRequestAdaptor<TResponse> : RawUnityWebRequestAdaptor<MultipartFileUnityWebRequestAdaptor<TResponse>, TResponse>
    {
        public byte[] Data => WebRequest.uploadHandler.data;
        protected override void SetAdaptedContentType(ContentType? contentType)
        {
            if (contentType is not null && string.IsNullOrEmpty(contentType.Value.Boundary))
                contentType = contentType.Value.With(newBoundary: RequestComponents.ContentType.Commons.RandomBoundary);
            base.SetAdaptedContentType(contentType);
        }
    }
}