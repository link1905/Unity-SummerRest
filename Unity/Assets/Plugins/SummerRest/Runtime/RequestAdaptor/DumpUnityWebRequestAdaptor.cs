using UnityEngine.Networking;

namespace SummerRest.Runtime.RequestAdaptor
{
    internal class DumpUnityWebRequestAdaptor : UnityWebRequestAdaptor<DumpUnityWebRequestAdaptor, UnityWebRequest>
    {
        //private string _rawResponse;
        internal override UnityWebRequest BuildResponse()
        {
            return WebRequest;
        }
    }
}