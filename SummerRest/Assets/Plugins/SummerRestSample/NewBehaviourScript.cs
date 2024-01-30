using System.Linq;
using SummerRest.Runtime.Authenticate.Repositories;
using UnityEngine;
using SummerRest.Runtime.Requests;

namespace SummerRestSample
{
    using PostComment = JsonPlaceHolder.Comments.GetPost1Comments;
    using Get1 = JsonPlaceHolder.Posts.Get1;
    using Post = JsonPlaceHolder.Posts.Post;
    using NinjaGet = NinjaApiWithAuth.GetRandomWord;

    internal class SampleManager : MonoBehaviour
    {
        private readonly Get1 _getPost = Get1.Create();
        private readonly Post _postPost = Post.Create();
        private readonly PostComment _getPostCmt = PostComment.Create();
        private readonly NinjaGet _getNinja = NinjaGet.Create();

        #region Simple get data

        private void GetPost1Data()
        {
            Debug.LogFormat("Calling to {0} {1}", _getPost.AbsoluteUrl, _getPost.Method);
            StartCoroutine(_getPost.RequestCoroutine<PostData>(HandleGetPost1Response));
            StartCoroutine(_getPost.DetailedRequestCoroutine<PostData>(HandleGetPost1DetailedResponse));
        }

        private void HandleGetPost1Response(PostData obj)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _getPost.AbsoluteUrl, obj);
        }

        private void HandleGetPost1DetailedResponse(WebResponse<PostData> obj)
        {
            Debug.LogFormat("Detailed response from {0}: {1} \n{2}\n{3}", _getPost.AbsoluteUrl, obj.Data,
                obj.StatusCode, obj.ContentType.FormedContentType);
        }

        #endregion


        #region Change request body

        private void PostPostData()
        {
            Debug.LogFormat("Calling to {0} {1}", _postPost.AbsoluteUrl, _postPost.Method);
            // Change the request body before calling the endpoint
            _postPost.BodyData = new PostData
            {
                id = Random.Range(0, 100),
                body = "My post body",
                title = "My post title",
                userId = Random.Range(0, 100)
            };
            StartCoroutine(_postPost.RequestCoroutine<PostData>(HandlePostPostResponse));
            StartCoroutine(_postPost.DetailedRequestCoroutine<PostData>(HandlePostPostDetailedResponse));
        }

        private void HandlePostPostResponse(PostData obj)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _postPost.AbsoluteUrl, obj);
        }

        private void HandlePostPostDetailedResponse(WebResponse<PostData> obj)
        {
            Debug.LogFormat("Detailed response from {0}: {1} \n{2}\n{3}", _postPost.AbsoluteUrl, obj.Data,
                obj.StatusCode, obj.ContentType.FormedContentType);
        }

        #endregion

        #region Change request params

        public void GetPostComment(int postId)
        {
            // Set request param
            // Use SetSingleParam because this is a single param
            // Use AddParam(s)ToList if you plan to use list param
            _getPostCmt.Params.SetSingleParam("postId", postId.ToString());
            // You can change more properties eg. Headers, Url, Timeout...
            Debug.LogFormat("Calling to {0} {1}", _getPostCmt.AbsoluteUrl, _getPostCmt.Method);
            StartCoroutine(_getPostCmt.RequestCoroutine<PostCommentData[]>(HandleGetCommentResponse));
        }

        private void HandleGetCommentResponse(PostCommentData[] obj)
        {
            var res = string.Join("; ", obj.Select(e => e.ToString()).ToArray());
            Debug.LogFormat("Simple response from {0}: {1}", _getPostCmt.AbsoluteUrl, res);
        }

        #endregion

        #region Auth request
        public void GetAuthData()
        {
            // Please note that we can not change the auth appender in runtime (only AuthKey is mutable)
            // Store secret first because the editor value is useless in runtime
            // Do this step everytime your secret is changed eg. after logging in, after refreshing token... 
            ISecretRepository.Current.Save(_getNinja.AuthKey, "my secret value to call the endpoint");
            Debug.LogFormat("Calling to {0} {1}", _getPostCmt.AbsoluteUrl, _getPostCmt.Method);
            // The secret value will be appended to the request automatically (by querying ISecretRepository)
            StartCoroutine(_getPostCmt.RequestCoroutine<string>(HandleGetAuthResponse));
        }

        private void HandleGetAuthResponse(string data)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _getPost.AbsoluteUrl, data);
        }
        #endregion

        #region Get image request
        
        #endregion
        
        #region Request async
        
        #endregion
    }
}