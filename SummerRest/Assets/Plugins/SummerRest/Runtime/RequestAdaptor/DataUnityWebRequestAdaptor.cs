namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adapting a normal (XML, JSON) data UnityWebRequest
    /// </summary>
    internal class DataUnityWebRequestAdaptor<TResponse> : RawUnityWebRequestAdaptor<DataUnityWebRequestAdaptor<TResponse>, TResponse>
    {
    }
}