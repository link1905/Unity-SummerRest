using SummerRest.Runtime.RequestComponents;
using SummerRestSample.Managers;
using SummerRestSample.Models;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SummerRestSample.Views
{
    public class RequestsView : MonoBehaviour
    {
        [SerializeField] private SampleManager manager;
        [SerializeField] private Button getProductBtn;
        [SerializeField] private Button searchProductBtn;
        [SerializeField] private Button addProductBtn;
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button getImageByRequest;
        [SerializeField] private Button getIndependentImage;
        private void Awake()
        {
            getProductBtn.onClick.AddListener(() => manager.GetProductData(1));
            searchProductBtn.onClick.AddListener(() => manager.SearchProductData("phone"));
            addProductBtn.onClick.AddListener(() => manager.AddProductData(new Product
            {
                id = Random.Range(0, 1000),
                title = "SummerRest",
                description = "It is a wonderful plugin",
            }));
            loginBtn.onClick.AddListener(() => manager.DoLogin(new Account
            {
                username = "atuny0",
                password = "9uQFF1Lh"
            }));
            getImageByRequest.onClick.AddListener(() => manager.GetImageByPredefinedRequest(1, 1));
            getIndependentImage.onClick.AddListener(() => manager.GetImageByAbsoluteUrl("https://cdn.dummyjson.com/product-images/1/1.jpg"));
        }
    }
}