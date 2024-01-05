using Newtonsoft.Json;
using SummerRest.Scripts.Utilities.DataStructures;
using SummerRest.Scripts.Utilities.RequestComponents;
using UnityEngine;

namespace SummerRest.Editor.Models
{
    public partial class Request : Endpoint 
    {
        [SerializeField, JsonIgnore] private HttpMethod method;
        [SerializeField, JsonIgnore] private KeyValue[] requestParams;
        [SerializeField, JsonIgnore] private RequestBody requestBody;
        public HttpMethod Method
        {
            get => method;
            private set => method = value;
        }
        public KeyValue[] RequestParams
        {
            get => requestParams;
            private set => requestParams = value;
        }
        public RequestBody RequestBody
        {
            get => requestBody;
            private set => requestBody = value;
        }

    }

#if UNITY_EDITOR
    public partial class Request
    {
        public override void Delete(bool fromParent)
        {
            if (fromParent && Parent is EndpointContainer parent)
                parent.Requests.Remove(this);
            base.Delete(fromParent);
        }

        [SerializeField, JsonIgnore] private Response latestResponse;
        [JsonIgnore] public Response LatestResponse
        {
            get => latestResponse;
            set => latestResponse = value;
        }
        public override string TypeName => nameof(Request);
    }
#endif
}
