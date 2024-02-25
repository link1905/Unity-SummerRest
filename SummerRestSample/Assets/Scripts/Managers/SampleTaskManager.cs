using System;
using Models;
using SummerRest.Runtime.Authenticate;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.RequestAdaptor;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using Views;
using DummyJson = SummerRest.Runtime.Requests.DummyJson;

namespace Managers
{
    using GetProduct = DummyJson.Products.GetProduct;
    using SearchProduct = DummyJson.Products.SearchProduct;
    using AddProduct = DummyJson.Products.AddProductData;
    using Login = DummyJson.Auths.Login;
    using GetProductImage = DummyJsonCdn.GetProductImage;
    using GetUser = DummyJson.Auths.GetUser;
#if SUMMER_REST_TASK
    using Cysharp.Threading.Tasks;
#endif
    internal class SampleTaskManager : SampleManager
    {
        [SerializeField] private ResponseView responseView;
        #region Simple get data
        private readonly GetProduct _getProduct = GetProduct.Create();
        public override void GetProductData(int productId)
#if !SUMMER_REST_TASK
        { }
#else 
        {
            // It is better not to use "async void", more details at https://github.com/Cysharp/UniTask?tab=readme-ov-file#async-void-vs-async-unitaskvoid
            GetProductDataAsync(productId).Forget();
        }
        private async UniTaskVoid GetProductDataAsync(int productId)
        {
            // Current format is https://dummyjson.com/products/{0}
            // Replace the placeholder with productId
            _getProduct.SetUrlValue(GetProduct.Keys.UrlFormat.ProductId, productId.ToString());
            responseView.StartCall(_getProduct.AbsoluteUrl, _getProduct.Method);
            // Simple response
            try
            {
                var product = await _getProduct.DataRequestAsync<Product>();
                responseView.SetResponse(product);
            }
            catch (ResponseErrorException responseErrorException)
            {
                responseView.SetError(responseErrorException.Error);
            }
            catch (Exception e)
            {
                //Undefined exception
                Debug.LogException(e);
            }
        }
#endif  
        #endregion
        
        #region Change request params
        private readonly SearchProduct _searchProduct = SearchProduct.Create();
        public override void SearchProductData(string category)
#if !SUMMER_REST_TASK
        { }
#else 
        {
            // It is better not to use "async void", more details at https://github.com/Cysharp/UniTask?tab=readme-ov-file#async-void-vs-async-unitaskvoid
            SearchProductDataAsync(category).Forget();
        }
        private async UniTaskVoid SearchProductDataAsync(string category)
        {
            try
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
                // Detailed response
                try
                {
                    // Please remember to wrap response in a using statement or call Dispose
                    using var response = await _searchProduct.DetailedDataRequestAsync<ProductPaging>();
                    responseView.SetResponse(response);
                }
                catch (ResponseErrorException responseErrorException)
                {
                    responseView.SetError(responseErrorException.Error);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            catch (ResponseErrorException e)
            {
                responseView.SetError(e.Error);
            }
        }
#endif  
        #endregion
        
        
        #region Post request body
        // This is an POST request (it's method has been generated, you do not need to set it manually)
        private readonly AddProduct _addProduct = AddProduct.Create();
        public override void AddProductData(Product product)
#if !SUMMER_REST_TASK
        { }
#else 
        {
            // It is better not to use "async void", more details at https://github.com/Cysharp/UniTask?tab=readme-ov-file#async-void-vs-async-unitaskvoid
            AddProductAsync(product).Forget();
        }
        private async UniTaskVoid AddProductAsync(Product product)
        {
            try
            {
                responseView.StartCall(_addProduct.AbsoluteUrl, _addProduct.Method);
                _addProduct.BodyData = product;
                // Please remember to wrap response in a using statement or call Dispose
                using var response = await _addProduct.DetailedDataRequestAsync<Product>();
                responseView.SetResponse(response);
            }
            catch (ResponseErrorException e)
            {
                responseView.SetError(e.Error);
            }
        }
#endif  
        #endregion

        
        #region Auth request

        private readonly Login _login = Login.Create();
        public override void DoLogin(Account account)
#if !SUMMER_REST_TASK
        { }
#else 
        {
            // It is better not to use "async void", more details at https://github.com/Cysharp/UniTask?tab=readme-ov-file#async-void-vs-async-unitaskvoid
            LoginAsync(account).Forget();
        }

        private async UniTaskVoid LoginAsync(Account account)
        {
            try
            {
                // Change the request body (Account data)
                _login.BodyData = account;
                Debug.Log("Start to login");
                responseView.StartCall(_login.AbsoluteUrl, _login.Method);
                var loginResponse = await _login.DataRequestAsync<LoginResponse>();
                await HandleAfterLogging(loginResponse);
            }
            catch (ResponseErrorException e)
            {
                Debug.LogError("Failed to login");
                responseView.SetError(e.Error);
            }
        }

        private UniTask HandleAfterLogging(LoginResponse response)
        {
            Debug.LogFormat("Received {0} token of user {1} from the login endpoint", response.token, response.id);
            
            // Set the received token into the secret repos
            // Store this secret first because the editor value is useless in runtime
            // Do this step everytime your secret is changed eg. after logging in, after refreshing token... 
            ISecretRepository.Current.Save(AuthKeys.DummyJsonToken, response.token);
            
            // Get user data after logging
            return GetAuthDataAsync();
        }
        // This is an authenticated endpoint
        // If you view the source code of this class, you may observe the constructor has IRequestModifier<AuthRequestModifier<SummerRestSample.DummyJsonApiAuthAppender, System.String>>.GetSingleton()
        // Please note that we can not change the auth appender in runtime (only AuthKey is mutable)
        private readonly GetUser _getUser = GetUser.Create();
        private async UniTask GetAuthDataAsync()
        {
            responseView.StartCall(_getUser.AbsoluteUrl, _getUser.Method);
            // Behind the scene, the secret value will be appended to the request automatically (by querying ISecretRepository)
            // You may debug the IAuthAppender if have any error
            var rawUserData = await _getUser.DataRequestAsync<string>();
            responseView.SetResponse(rawUserData);
        }  
#endif

        #endregion

        // This section shows a simple guidance on downloading images
        // Requesting audio clip is similar to this 
        #region Get image request

        private readonly GetProductImage _getProductImage = GetProductImage.Create();
        /// <summary>
        /// You should leverage generated classes in case you know the structure of the image beforehand
        /// </summary>
        public override void GetImageByPredefinedRequest(int productId, int imgOrder)
#if !SUMMER_REST_TASK
        { }
#else 
        {
            // It is better not to use "async void", more details at https://github.com/Cysharp/UniTask?tab=readme-ov-file#async-void-vs-async-unitaskvoid
            GetImageByPredefinedRequestAsync(productId, imgOrder).Forget();
        }

        private async UniTaskVoid GetImageByPredefinedRequestAsync(int productId, int imgOrder)
        {
            _getProductImage.SetUrlValue(GetProductImage.Keys.UrlFormat.ProductId, productId.ToString());
            _getProductImage.SetUrlValue(GetProductImage.Keys.UrlFormat.ImageOrder, imgOrder.ToString());
            responseView.StartCall(_getProductImage.AbsoluteUrl, _getProductImage.Method);
            // Please remember to wrap response in a using statement or call Dispose
            using var img = await _getProductImage.DetailedTextureRequestAsync(false);
            responseView.SetImageResponse(img);
        }
#endif

        // In case you receive an absolute url and you do not have a predefined matching class
        public override void GetImageByAbsoluteUrl(string url)
#if !SUMMER_REST_TASK
        { }
#else 
        {
            // It is better not to use "async void", more details at https://github.com/Cysharp/UniTask?tab=readme-ov-file#async-void-vs-async-unitaskvoid
            GetImageByAbsoluteUrlAsync(url).Forget();
        }

        private async UniTaskVoid GetImageByAbsoluteUrlAsync(string url)
        {
            var texture = await WebRequestUtility.TextureRequestAsync(url, false, 
                // Use a builder to modify the request
                adaptorBuilder: b =>
                {
                    responseView.StartCall(b.Url, b.Method);
                    // Because this is a independent request, so you must adapt the request data on your own
                    // b.RedirectLimit = 3;
                    b.SetHeader("my-header", "my-value");
                });
            responseView.SetImageResponse(texture);
        }
#endif
        #endregion
    }
}