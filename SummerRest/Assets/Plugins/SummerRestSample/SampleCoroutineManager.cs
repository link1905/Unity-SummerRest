using System.Linq;
using SummerRest.Runtime.Authenticate;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using DummyJson = SummerRest.Runtime.Requests.DummyJson;

namespace SummerRestSample
{
    using GetProduct = DummyJson.Products.GetProduct;
    using SearchProduct = DummyJson.Products.SearchProduct;
    using AddProduct = DummyJson.Products.AddProductData;
    using Login = DummyJson.Auth.Login;
    using GetUser = DummyJson.Auth.GetUser;
    using GetProductImage = DummyJsonCdn.GetProductImage;

    internal class SampleCoroutineManager : MonoBehaviour
    {
        #region Simple get data
        private readonly GetProduct _getProduct = GetProduct.Create();
        public void GetProductData(string productId)
        {
            // Current format is https://dummyjson.com/products/{0}
            // Replace the placeholder with productId
            _getProduct.SetUrlValue(GetProduct.Keys.UrlFormat.ProductId, productId);
            
            Debug.LogFormat("Calling to {0} {1}", _getProduct.AbsoluteUrl, _getProduct.Method);
            
            // Simple response
            StartCoroutine(_getProduct.DataRequestCoroutine<Product>(HandleGetProductResponse));
        }
        private void HandleGetProductResponse(Product obj)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _getProduct.AbsoluteUrl, obj);
            
            // Detailed response
            StartCoroutine(_getProduct.DetailedRequestCoroutine<Product>(HandleGetProductDetailedResponse));
        }
        private void HandleGetProductDetailedResponse(WebResponse<Product> obj)
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
            StartCoroutine(_searchProduct.DataRequestCoroutine<Product[]>(HandleSearchProductResponse));
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
        public void AddProductData(Product product)
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
            
            StartCoroutine(_addProduct.DetailedRequestCoroutine<Product>(HandleAddProductDetailedResponse));
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
            Debug.Log("Start to login");
            StartCoroutine(_login.DataRequestCoroutine<LoginResponse>(HandleAfterLogging));
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
            StartCoroutine(_getUser.DataRequestCoroutine<string>(HandleGetAuthResponse));
        }
        private void HandleGetAuthResponse(string data)
        {
            Debug.LogFormat("Simple response from {0}: {1}", _getProduct.AbsoluteUrl, data);
        }
        #endregion

        // This section shows a simple guidance on downloading images
        // Requesting audio clip is similar to this 
        #region Get image request

        private readonly GetProductImage _getProductImage;
        /// <summary>
        /// You should leverage generated classes in case you know the structure of the image beforehand
        /// </summary>
        public void GetImageByPredefinedRequest(int productId, int imgOrder)
        {
            _getProductImage.SetUrlValue(GetProductImage.Keys.UrlFormat.ProductId, productId.ToString());
            _getProductImage.SetUrlValue(GetProductImage.Keys.UrlFormat.ImageOrder, imgOrder.ToString());
            StartCoroutine(_getProductImage.TextureRequestCoroutine(false, ShowImage));
        }

        // In case you receive an absolute url and you do not have a predefined matching class
        public void GetImageByAbsoluteUrl()
        {
            StartCoroutine(WebRequestUtility.TextureRequestCoroutine( "https://cdn.dummyjson.com/product-images/1/1.jpg", false, ShowImage));
        }

        private void ShowImage(Texture2D texture2D)
        {
            
        }
        #endregion

    }
}