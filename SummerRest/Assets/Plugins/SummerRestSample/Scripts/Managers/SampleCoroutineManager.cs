using SummerRest.Runtime.Authenticate;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using SummerRestSample.Models;
using SummerRestSample.Views;
using UnityEngine;
using DummyJson = SummerRest.Runtime.Requests.DummyJson;

namespace SummerRestSample.Managers
{
    using GetProduct = DummyJson.Products.GetProduct;
    using SearchProduct = DummyJson.Products.SearchProduct;
    using AddProduct = DummyJson.Products.AddProductData;
    using Login = DummyJson.Auth.Login;
    using GetUser = DummyJson.Auth.GetUser;
    using GetProductImage = DummyJsonCdn.GetProductImage;

    internal class SampleCoroutineManager : SampleManager
    {
        [SerializeField] private ResponseView responseView;
        
        #region Simple get data
        private readonly GetProduct _getProduct = GetProduct.Create();
        public override void GetProductData(int productId)
        {
            // Current format is https://dummyjson.com/products/{0}
            // Replace the placeholder with productId
            _getProduct.SetUrlValue(GetProduct.Keys.UrlFormat.ProductId, productId.ToString());
            responseView.StartCall(_getProduct.AbsoluteUrl, _getProduct.Method);
            // Simple response
            StartCoroutine(_getProduct.DataRequestCoroutine<Product>(responseView.SetResponse));
        }
        
        #endregion
        
        #region Change request params
        private readonly SearchProduct _searchProduct = SearchProduct.Create();
        public override void SearchProductData(string category)
        {
            // Set request param
            // Use SetSingleParam because this is a single param (Use AddParam(s)ToList if you plan to use list param)
            _searchProduct.Params.SetSingleParam(SearchProduct.Keys.Params.Q, category);
            // Current select param (based on the editor) is title,price
            // Add "category" => title,price,category
            _searchProduct.Params.AddParamToList(SearchProduct.Keys.Params.Select, "category");
            // You can change more properties eg. Headers, Url, Timeout...
            _searchProduct.Headers[SearchProduct.Keys.Headers.MyHeader] = "my-header-value";
            _searchProduct.TimeoutSeconds = 3;

            responseView.StartCall(_searchProduct.AbsoluteUrl, _searchProduct.Method);
            // Simple request
            StartCoroutine(_searchProduct.DetailedDataRequestCoroutine<Product[]>(HandleSearchProductResponse, 
                responseView.SetError));
        }
        private void HandleSearchProductResponse(WebResponse<Product[]> response)
        {
            responseView.SetResponse(response);
        }
        
        #endregion
        
        
        #region Post request body
        // This is an POST request (it's method has been generated, you do not need to set it manually)
        private readonly AddProduct _addProduct = AddProduct.Create();
        public override void AddProductData(Product product)
        {
            responseView.StartCall(_addProduct.AbsoluteUrl, _addProduct.Method);
            // Change the request body before calling the endpoint
            _addProduct.BodyData = product;
            StartCoroutine(_addProduct.DetailedDataRequestCoroutine<Product>(responseView.SetResponse, responseView.SetError));
        }
        #endregion

        
        #region Auth request

        private readonly Login _login = Login.Create();
        public override void DoLogin(Account account)
        {
            // Change the request body (Account data)
            _login.BodyData = account;
            Debug.Log("Start to login");
            responseView.StartCall(_login.AbsoluteUrl, _login.Method);
            StartCoroutine(_login.DataRequestCoroutine<LoginResponse>(HandleAfterLogging, OnLoginFailed));
        }
        private void OnLoginFailed(ResponseError error)
        {
            Debug.LogError("Failed to login");
            responseView.SetError(error);
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
            responseView.StartCall(_getUser.AbsoluteUrl, _getUser.Method);
            // Behind the scene, the secret value will be appended to the request automatically (by querying ISecretRepository)
            // You may debug the IAuthAppender if have any error
            StartCoroutine(_getUser.DataRequestCoroutine<string>(responseView.SetResponse));
        }
        #endregion

        // This section shows a simple guidance on downloading images
        // Requesting audio clip is similar to this 
        #region Get image request

        private readonly GetProductImage _getProductImage;
        /// <summary>
        /// You should leverage generated classes in case you know the structure of the image beforehand
        /// </summary>
        public override void GetImageByPredefinedRequest(int productId, int imgOrder)
        {
            _getProductImage.SetUrlValue(GetProductImage.Keys.UrlFormat.ProductId, productId.ToString());
            _getProductImage.SetUrlValue(GetProductImage.Keys.UrlFormat.ImageOrder, imgOrder.ToString());
            responseView.StartCall(_getProductImage.AbsoluteUrl, _getProductImage.Method);
            StartCoroutine(_getProductImage.DetailedTextureRequestCoroutine(false, responseView.SetImageResponse));
        }

        // In case you receive an absolute url and you do not have a predefined matching class
        public override void GetImageByAbsoluteUrl(string url)
        {
            StartCoroutine(WebRequestUtility.TextureRequestCoroutine(url, false, 
                responseView.SetImageResponse, adaptorBuilder:
                // Use a builder to modify the request
                b =>
                {
                    responseView.StartCall(b.Url, b.Method);
                    // Change request data
                    // b.RedirectLimit = 3;
                    // b.SetHeader("my-header", "my-value");
                }));
        }
        #endregion

    }
}