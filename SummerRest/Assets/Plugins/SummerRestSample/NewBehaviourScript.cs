using System.Collections.Generic;
using System.IO;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Authenticate.Appenders;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRestSample
{
    public class NewBehaviourScript : MonoBehaviour
    {
        [SerializeField, ClassTypeConstraint(typeof(IAuthAppender<,>))] 
        private ClassTypeReference appenderType = new(typeof(BearerTokenAuthAppender));

        [SerializeReference] private IRequestBodyData bodyData = new PostData();

        public void A()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            // var bytes = File.ReadAllBytes(datapath);
            // string filename = Path.GetFileName(datapath);
            // formData.Add(new MultipartFormFileSection(filename, bytes));
            byte[] boundary = UnityWebRequest.GenerateBoundary();
            UnityWebRequest content = UnityWebRequest.Post("", formData, boundary);
            content.method = "PATCH";
        }
    }
}
//
// namespace Another
// {
//     using api = SummerRest.Samples.Behaviours.NinjaApiAuthAppender;
// }
//
// namespace SummerRest.Samples.Behaviours
// {
//
//     using Request2 = MyDomain.MyService.MyRequest2;
//     public class MyResponseData
//     {
//         public string Name { get; set; }
//         public int Age { get; set; }
//     }
//     public class MyBehaviour : MonoBehaviour
//     {
//         private Request2 _myRequest;
//         private void DoRequest()
//         {
//             // Request normal data
//             StartCoroutine(_myRequest.RequestCoroutine<MyResponseData>(HandleResponse, HandleError));
//             // Request texture
//             StartCoroutine(_myRequest.TextureRequestCoroutine(HandleResponseTexture, true));
//             // Request audio clip
//             StartCoroutine(_myRequest.AudioRequestCoroutine(HandleResponseAudioClip, AudioType.WAV));
//         }
//
//         private void DoDetailedRequest()
//         {
//             // Request normal data
//             StartCoroutine(_myRequest.DetailedRequestCoroutine<MyResponseData>(HandleDetailedResponse));
//             // Request audioClip/texture is similar to the previous step 
//         }
//         private void HandleDetailedResponse(WebResponse<MyResponseData> responseData)
//         {
//             Debug.Log(responseData.StatusCode);
//             Debug.Log(responseData.Error);
//             Debug.Log(responseData.RawData);
//             Debug.Log(responseData.Data);
//             ...
//         }
//         
//         
//         private void HandleResponse(MyResponseData responseData) { ... }
//         private void HandleResponseTexture(Texture2D texture) { ... }
//         private void HandleResponseAudioClip(AudioClip audioClip) { ... }
//         // OnError is optional, you should consider detailed calls when encountering complex errors
//         private void HandleError(string error) { ... }
//
//         public class User
//         {
//             public string UserId { get; set; }
//         }
//         // Generated classes
//         private UserDataRequest _userRequest;
//         private TextureRequest _imageRequest;
//         private async UniTask LoadUserData()
//         {
//             var userData = await _userRequest.RequestAsync<User>();
//             _imageRequest.Params.AddParam("user-id", userData.UserId);
//             var userIcon = await _imageRequest.TextureRequestAsync();
//         }
//     }
//
//     public class MyRequestData
//     {
//         public string Name { get; set; }
//         public int Age { get; set; }
//     }
//     public class NinjaApiAuthAppender : IAuthAppender<NinjaApiAuthAppender>
//     {
//         public void Append<TResponse>(string authDataKey, IWebRequestAdaptor<TResponse> requestAdaptor)
//         {
//             //Get auth token 
//             var token = IAuthDataRepository.Current.Get<string>(authDataKey);
//             //Append it into the request's header
//             requestAdaptor.SetHeader("X-Api-Key", token);
//         }
//     }
// [Serializable]
//     internal class MyRequestBody : IRequestBodyData
//     {
//         // is not serialized by NewtonSoft 
//         [SerializeField] private int notSerializedFieldBecausePrivate; 
//         // does not show up on the Inspector 
//         public int NotExposedBecauseUnityDoesNotRecognizeProperty { get; set; }
//         //public => will be serialized
//         //[field: SerializeField] => make the backing field be shown 
//         [field: SerializeField] public int ExposedAndSerializedBecausePublishAndUnityRecognizeTheBackingField { get; set; }
//     }
//     public class MyRequestBody1 : IRequestBodyData {} 
//     public class NewBehaviourScript : MonoBehaviour
//     {
//         private class MyRequestBody2 : IRequestBodyData {} 
//         [SerializeField][ClassTypeConstraint(typeof(IRequestBodyData))] 
//         private ClassTypeReference classTypeReference;
//     }
// }
