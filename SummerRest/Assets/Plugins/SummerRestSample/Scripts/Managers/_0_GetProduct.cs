
using Cysharp.Threading.Tasks;
using SummerRest.Runtime.Requests;
using SummerRestSample.Views;
using UnityEngine;
using UnityEngine.UI;

namespace SummerRestSample.Managers
{
    using GetProduct = DummyJson.Products.GetProduct;
    public class _0_GetProduct : BaseRequestManager
    {
        [SerializeField] private InputField productIdInputField;
        [SerializeField] private ProductView productView;
        
        private readonly GetProduct _getProduct = GetProduct.Create();

        public void GetProductData(int productId)
        {
            // Current format is https://dummyjson.com/products/{0}
            // Replace the placeholder with productId
            _getProduct.SetUrlValue(GetProduct.Keys.UrlFormat.ProductId, productId.ToString());
            Debug.LogFormat("Calling to {0} {1}", _getProduct.AbsoluteUrl, _getProduct.Method);
            // Simple response
            StartCoroutine(_getProduct.DataRequestCoroutine<Product>(productView.ShowProduct));
            // Uncomment to make a detailed response
            // StartCoroutine(_getProduct.DetailedRequestCoroutine<Product>(response =>
            // {
            //     Debug.LogFormat("Detailed response from {0}: {{{1}, {2}}}", _getProduct.AbsoluteUrl, 
            //         response.StatusCode, response.ContentType.FormedContentType);
            //     productView.ShowProduct(response.Data);
            // }));
        }
        protected override void OnCoroutineClicked()
        {
            if (!int.TryParse(productIdInputField.text, out var result))
                result = 1;
            GetProductData(result);
        }
        protected override void OnTaskClicked()
        {
        }
        
#if SUMMER_REST_TASK
        private async UniTaskVoid GetProductDataAsync(int productId)
        {
            // Current format is https://dummyjson.com/products/{0}
            // Replace the placeholder with productId
            _getProduct.SetUrlValue(GetProduct.Keys.UrlFormat.ProductId, productId.ToString());
            Debug.LogFormat("Calling to {0} {1}", _getProduct.AbsoluteUrl, _getProduct.Method);
            // Simple response
            var prod = await _getProduct.DataRequestAsync<Product>();
            productView.ShowProduct(prod);
            // Detailed response
            StartCoroutine(_getProduct.DetailedRequestCoroutine<Product>(response =>
            {
                Debug.LogFormat("Detailed response from {0}: {{{1}, {2}}}", _getProduct.AbsoluteUrl, 
                    response.StatusCode, response.ContentType.FormedContentType);
                productView.ShowProduct(response.Data);
            }));
        }
#endif
    }
}