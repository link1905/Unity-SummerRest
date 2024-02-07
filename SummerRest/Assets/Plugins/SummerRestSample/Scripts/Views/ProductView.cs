using UnityEngine;
using UnityEngine.UI;

namespace SummerRestSample.Views
{
    public class ProductView : MonoBehaviour
    {
        [SerializeField] private Text txtId;
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtDescription;
        public void ShowProduct(Product product)
        {
            txtId.text = product.id.ToString();
            txtTitle.text = product.title;
            txtDescription.text = product.description;
        }
    }
}