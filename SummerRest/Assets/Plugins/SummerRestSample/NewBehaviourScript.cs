using System;
using System.Linq;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.Requests;
using UnityEngine;
using DummyJson = SummerRest.Runtime.Requests.DummyJson;
using Random = UnityEngine.Random;

namespace SummerRestSample
{
    using GetProduct1 = DummyJson.Products.GetProduct1;
    using SearchProduct = DummyJson.Products.SearchProduct;
    internal class SampleManager : MonoBehaviour
    {
        private void OnValidate()
        {
        }
        // private readonly GetProduct1 _getProduct1 = GetProduct1.Create();
        // private readonly SearchProduct _searchProduct = SearchProduct.Create();
        //
        //
        // private readonly Post _postPost = Post.Create();
        // private readonly PostComment _getPostCmt = PostComment.Create();
        // private readonly NinjaGet _getNinja = NinjaGet.Create();
        //
        // #region Simple get data
        // private void GetProduct1Data()
        // {
        //     Debug.LogFormat("Calling to {0} {1}", _getProduct1.AbsoluteUrl, _getProduct1.Method);
        //     StartCoroutine(_getProduct1.RequestCoroutine<Product>(HandleGetProduct1Response));
        //     StartCoroutine(_getProduct1.DetailedRequestCoroutine<Product>(HandleGetProduct1DetailedResponse));
        // }
        // private void HandleGetProduct1Response(Product obj)
        // {
        //     Debug.LogFormat("Simple response from {0}: {1}", _getProduct1.AbsoluteUrl, obj);
        // }
        // private void HandleGetProduct1DetailedResponse(WebResponse<Product> obj)
        // {
        //     Debug.LogFormat("Detailed response from {0}: {{{1}, {2}, {3}}}", _getProduct1.AbsoluteUrl, 
        //         obj.StatusCode, obj.ContentType.FormedContentType, obj.Data);
        // }
        //
        // #endregion
        //
        //
        // #region Change request body
        //
        // private void PostPostData()
        // {
        //     Debug.LogFormat("Calling to {0} {1}", _postPost.AbsoluteUrl, _postPost.Method);
        //     // Change the request body before calling the endpoint
        //     _postPost.BodyData = new PostData
        //     {
        //         id = Random.Range(0, 100),
        //         body = "My post body",
        //         title = "My post title",
        //         userId = Random.Range(0, 100)
        //     };
        //     StartCoroutine(_postPost.RequestCoroutine<PostData>(HandlePostPostResponse));
        //     StartCoroutine(_postPost.DetailedRequestCoroutine<PostData>(HandlePostPostDetailedResponse));
        // }
        //
        // private void HandlePostPostResponse(PostData obj)
        // {
        //     Debug.LogFormat("Simple response from {0}: {1}", _postPost.AbsoluteUrl, obj);
        // }
        //
        // private void HandlePostPostDetailedResponse(WebResponse<PostData> obj)
        // {
        //     Debug.LogFormat("Detailed response from {0}: {1} \n{2}\n{3}", _postPost.AbsoluteUrl, obj.Data,
        //         obj.StatusCode, obj.ContentType.FormedContentType);
        // }
        //
        // #endregion
        //
        // #region Change request params
        //
        // public void GetPostComment(int postId)
        // {
        //     // Set request param
        //     // Use SetSingleParam because this is a single param
        //     // Use AddParam(s)ToList if you plan to use list param
        //     _searchProduct.Params.SetSingleParam("q", postId.ToString());
        //     // You can change more properties eg. Headers, Url, Timeout...
        //     Debug.LogFormat("Calling to {0} {1}", _getPostCmt.AbsoluteUrl, _getPostCmt.Method);
        //     StartCoroutine(_getPostCmt.RequestCoroutine<PostCommentData[]>(HandleGetCommentResponse));
        // }
        //
        // private void HandleGetCommentResponse(PostCommentData[] obj)
        // {
        //     var res = string.Join("; ", obj.Select(e => e.ToString()).ToArray());
        //     Debug.LogFormat("Simple response from {0}: {1}", _getPostCmt.AbsoluteUrl, res);
        // }
        //
        // #endregion
        //
        // #region Auth request
        // public void GetAuthData()
        // {
        //     // Please note that we can not change the auth appender in runtime (only AuthKey is mutable)
        //     // Store secret first because the editor value is useless in runtime
        //     // Do this step everytime your secret is changed eg. after logging in, after refreshing token... 
        //     ISecretRepository.Current.Save(_getNinja.AuthKey, "my secret value to call the endpoint");
        //     Debug.LogFormat("Calling to {0} {1}", _getPostCmt.AbsoluteUrl, _getPostCmt.Method);
        //     // The secret value will be appended to the request automatically (by querying ISecretRepository)
        //     StartCoroutine(_getPostCmt.RequestCoroutine<string>(HandleGetAuthResponse));
        // }
        //
        // private void HandleGetAuthResponse(string data)
        // {
        //     Debug.LogFormat("Simple response from {0}: {1}", _getProduct1.AbsoluteUrl, data);
        // }
        // #endregion
        //
        // #region Get image request
        //
        // #endregion
        //
        // #region Request async
        //
        // #endregion
    }
}