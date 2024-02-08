using SummerRest.Runtime.RequestComponents;
using SummerRestSample.Models;
using UnityEngine;

namespace SummerRestSample.Managers
{
    internal abstract class SampleManager : MonoBehaviour
    {
        public abstract void GetProductData(int productId);
        public abstract void SearchProductData(string category);
        public abstract void AddProductData(Product product);
        public abstract void DoLogin(Account account);
        public abstract void GetImageByPredefinedRequest(int productId, int imgOrder);
        public abstract void GetImageByAbsoluteUrl(string url);
    }
}