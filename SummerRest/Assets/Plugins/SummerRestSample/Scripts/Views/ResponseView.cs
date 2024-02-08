using System.Net;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace SummerRestSample.Views
{
    public class ResponseView : MonoBehaviour
    {
        [SerializeField] private Text urlTxt;
        [SerializeField] private Text responseCodeTxt;
        [SerializeField] private Text responseBodyTxt;
        [SerializeField] private Text errorTxt;
        [SerializeField] private Image image;
        public void StartCall(string url, HttpMethod httpMethod)
        {
            urlTxt.text = $"Calling to {url} {httpMethod}";
            Clear();
        }
        public void SetResponse<T>(T data)
        {
            responseBodyTxt.text = IDataSerializer.Current.Serialize(data, DataFormat.Json);
        }
        public void SetResponse<T>(WebResponse<T> webResponse)
        {
            responseCodeTxt.text = webResponse.StatusCode.ToString();
            SetResponse(webResponse.Data);
        }
        public void SetImageResponse(Texture2D texture2D)
        {
            image.sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        }
        public void SetImageResponse(WebResponse<Texture2D> texture2D)
        {
            responseCodeTxt.text = texture2D.StatusCode.ToString();
            SetImageResponse(texture2D.Data);
        }
        public void SetError(ResponseError error)
        {
            errorTxt.text = error.Message;
            responseBodyTxt.text = error.ErrorBody;
            responseCodeTxt.text = error.StatusCode.ToString();
        }
        public void Clear()
        {
            responseCodeTxt.text = string.Empty;
            responseBodyTxt.text = string.Empty;
            errorTxt.text = string.Empty;
            image.sprite = null;
        } 
    }
}