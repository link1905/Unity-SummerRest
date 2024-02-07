using System;
using System.Linq;
using SummerRest.Editor.Models;
using SummerRest.Runtime.Authenticate;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using DummyJson = SummerRest.Runtime.Requests.DummyJson;
using Random = UnityEngine.Random;

namespace SummerRestSample
{
    using GetProduct = DummyJson.Products.GetProduct;
    using SearchProduct = DummyJson.Products.SearchProduct;
    using AddProduct = DummyJson.Products.AddProductData;
    using Login = DummyJson.Auth.Login;
    using GetUser = DummyJson.Auth.GetUser;
    
    internal class SampleManager : MonoBehaviour
    {
        #region Simple get data
        private readonly GetProduct _getProduct = GetProduct.Create();
        private void GetProductData(string productId)
        {
            // Current format is https://dummyjson.com/products/{0}
            // Replace the placeholder with productId
            _getProduct.SetUrlValue(GetProduct.Keys.UrlFormat.ProductId, productId);
            
            Debug.LogFormat("Calling to {0} {1}", _getProduct.AbsoluteUrl, _getProduct.Method);
            
            // Simple response
            StartCoroutine(_getProduct.RequestCoroutine<Product>(HandleGetProduct1Response));
            // Detailed response
            StartCoroutine(_getProduct.DetailedRequestCoroutine<Product>(HandleGetProduct1DetailedResponse));
        }
        private void HandleGetProduct1Response(Product obj)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _getProduct.AbsoluteUrl, obj);
        }
        private void HandleGetProduct1DetailedResponse(WebResponse<Product> obj)
        {
            Debug.LogFormat("Detailed response from {0}: {{{1}, {2}, {3}}}", _getProduct.AbsoluteUrl, 
                obj.StatusCode, obj.ContentType.FormedContentType, obj.Data);
        }
        
        #endregion
        
                
        #region Change request params
        private readonly SearchProduct _searchProduct = SearchProduct.Create();
        public void SearchProductData(string productId)
        {
            // Set request param
            // Use SetSingleParam because this is a single param (Use AddParam(s)ToList if you plan to use list param)
            _searchProduct.Params.SetSingleParam(SearchProduct.Keys.Params.Q, productId);
            // You can change more properties eg. Headers, Url, Timeout...
            Debug.LogFormat("Calling to {0} {1}", _searchProduct.AbsoluteUrl, _searchProduct.Method);
            // Simple request
            StartCoroutine(_searchProduct.RequestCoroutine<Product[]>(HandleSearchProductResponse));
        }
        
        private void HandleSearchProductResponse(Product[] obj)
        {
            var res = string.Join("; ", obj.Select(e => e.ToString()).ToArray());
            Debug.LogFormat("Simple response from {0}: {1}", _searchProduct.AbsoluteUrl, res);
        }
        
        #endregion
        
        
        #region Post request body
        // This is an POST request (it's method has been generated, you do not need to set it manually)
        private readonly AddProduct _addProduct = AddProduct.Create();
        private void AddProductData(Product product)
        {
            Debug.LogFormat("Calling to {0} {1}", _addProduct.AbsoluteUrl, _addProduct.Method);
            // _addProduct.BodyData = new Product
            // {
            //     id = Random.Range(0, 100),
            //     title = "My product",
            //     description = "Wonderful product",
            // };
            // Change the request body before calling the endpoint
            _addProduct.BodyData = product;
            
            StartCoroutine(_addProduct.RequestCoroutine<Product>(HandleAddProductResponse));
            
            StartCoroutine(_addProduct.DetailedRequestCoroutine<Product>(HandleAddProductDetailedResponse));
        }
        
        private void HandleAddProductResponse(Product obj)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _addProduct.AbsoluteUrl, obj);
        }
        
        private void HandleAddProductDetailedResponse(WebResponse<Product> obj)
        {
            Debug.LogFormat("Detailed response from {0}: {1} \n{2}\n{3}", _addProduct.AbsoluteUrl, obj.Data,
                obj.StatusCode, obj.ContentType.FormedContentType);
        }
        
        #endregion

        
        #region Auth request

        private readonly Login _login = Login.Create();
        public void DoLogin(Account account)
        {
            // Change the request body (Account data)
            _login.BodyData = account;
            StartCoroutine(_login.RequestCoroutine<LoginResponse>(HandleAfterLogging));
        }
        private void HandleAfterLogging(LoginResponse response)
        {
            Debug.LogFormat("Received {0} token of user {1} from the login endpoint", response.token, response.id);
            
            // Set the received token into the secret repos
            // Store this secret first because the editor value is useless in runtime
            // Do this step everytime your secret is changed eg. after logging in, after refreshing token... 
            ISecretRepository.Current.Save(AuthKeys.DummyJsonToken, response.token);
            
            // Get user data after logging
            GetAuthData();
        }
        // This is an authenticated endpoint
        // If you view the source code of this class, you may observe the constructor has IRequestModifier<AuthRequestModifier<SummerRestSample.DummyJsonApiAuthAppender, System.String>>.GetSingleton()
        // Please note that we can not change the auth appender in runtime (only AuthKey is mutable)
        private readonly GetUser _getUser = GetUser.Create();
        public void GetAuthData()
        {
            Debug.LogFormat("Calling to {0} {1}", _getUser.AbsoluteUrl, _getUser.Method);
            // Behind the scene, the secret value will be appended to the request automatically (by querying ISecretRepository)
            // You may debug the IAuthAppender if have any error
            StartCoroutine(_getUser.RequestCoroutine<string>(HandleGetAuthResponse));
        }
        private void HandleGetAuthResponse(string data)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _getProduct.AbsoluteUrl, data);
        }
        #endregion
        
        #region Get image request
        
        #endregion
        
        #region Request async
#if SUMMER_REST_TASK

#endif
        #endregion
    }
}