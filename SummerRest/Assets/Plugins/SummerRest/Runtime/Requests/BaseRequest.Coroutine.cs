using System;
using System.Collections;
using System.Collections.Generic;
using SummerRest.Runtime.Extensions;
using SummerRest.Runtime.RequestAdaptor;
using UnityEngine;
using UnityEngine.Networking;

namespace SummerRest.Runtime.Requests
{
    public partial class BaseRequest<TRequest>
    {
 
        /// <summary>
        /// Simple <see cref="Texture2D"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator TextureRequestCoroutine(
            bool readable, 
            Action<Texture2D> doneCallback,
            Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.TextureRequestCoroutine(AbsoluteUrl, readable, doneCallback, errorCallback, SetRequestData);
        }
        /// <summary>
        /// Simple <see cref="AudioClip"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator AudioRequestCoroutine(
            AudioType audioType, Action<AudioClip> doneCallback, 
            Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.AudioRequestCoroutine(AbsoluteUrl, audioType, doneCallback, errorCallback, SetRequestData);
        }

        /// <summary>
        /// Detailed <see cref="Texture2D"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="readable">Texture response readable</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator DetailedTextureRequestCoroutine(
            bool readable, Action<IWebResponse<Texture2D>> doneCallback, Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.DetailedTextureRequestCoroutine(AbsoluteUrl, readable, doneCallback, errorCallback, SetRequestData);
        }
        /// <summary>
        /// Detailed <see cref="AudioClip"/> request using Unity coroutine with callbacks
        /// </summary>
        /// <param name="audioType">Type of the audio response</param>
        /// <param name="doneCallback">Invoked when the request is finished without an error</param>
        /// <param name="errorCallback">Invoked when the request is finished with an error</param>
        /// <returns></returns>
        public IEnumerator DetailedAudioRequestCoroutine(AudioType audioType, 
            Action<IWebResponse<AudioClip>> doneCallback, Action<ResponseError> errorCallback = null)
        {
            return WebRequestUtility.DetailedAudioRequestCoroutine(AbsoluteUrl, audioType, doneCallback, errorCallback, SetRequestData);
        }
    }
}