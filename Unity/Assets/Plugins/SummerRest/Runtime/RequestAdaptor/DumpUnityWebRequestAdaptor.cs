using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    /// <summary>
    /// Request adaptor returning the UnityWebRequest itself because of no hint to extract the result 
    /// </summary>
    internal class DumpUnityWebRequestAdaptor : UnityWebRequestAdaptor<DumpUnityWebRequestAdaptor, UnityWebRequest>
    {
        //private string _rawResponse;
        internal override UnityWebRequest BuildResponse()
        {
            return WebRequest;
        }
    }
}