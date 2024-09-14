using GameLogic.Base;
using GameLogic.Coroutiner;
using GameLogic.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using WebResponse = GameLogic.Service.WebResponse;

namespace GameLogic
{
    /// <summary>
    /// Web Request Module
    /// 网络请求模块
    /// </summary>
    public class WebRequestModule : SingletonMgr<WebRequestModule>
    {
        #region Variable
        private IWebService service;
        private IWebRequest sendRequest;
        private Dictionary<string, string> superHeaders;
        private Dictionary<IWebRequest, Coroutine> webRequests;
        #endregion

        public void Start()
        {
            Init(new WebService());
        }

        public void Init(IWebService service)
        {
            superHeaders = new Dictionary<string, string>();
            webRequests = new Dictionary<IWebRequest, Coroutine>();
            this.service = service;
        }

        public void Update()
        {
            if (webRequests != null && webRequests.Count > 0)
            {
                var keys = new List<IWebRequest>(webRequests.Keys);
                foreach (var httpRequest in keys)
                {
                    // Update the progress of the request
                    (httpRequest as IUpdateProgress)?.UpdateProgress();
                }
            }
        }

        #region Headers
        /// <summary>
        /// SuperHeaders are key value pairs, which will be added to every subsequent WebRequest.
        /// SuperHeaders是键值对，将被添加到每个后续的WebRequest中。
        /// </summary>
        /// <returns>A dictionary of super-headers.</returns>
        public Dictionary<string, string> GetSuperHeaders()
        {
            return new Dictionary<string, string>(superHeaders);
        }

        /// <summary>
        /// Set the header as a SuperHeader key value pair, and if the header key already exists, the value will be replaced.
        /// 将标头设置为SuperHeaders键值对，如果标头键已存在，则该值将被替换。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetSuperHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key cannot be null or empty");
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("The value cannot be null or empty. If you want to delete the value, please use the RemoveSuperHeader() method.");
                return;
            }

            superHeaders[key] = value;
        }

        /// <summary>
        /// Remove headers from the 'SuperHeaders' list
        /// 从“SuperHeaders”列表中删除标头
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveSuperHeader(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key cannot be null or empty");
            }

            return superHeaders.Remove(key);
        }
        #endregion

        #region Static Request Method
        public static IWebRequest Get(string uri)
        {
            return Instance.service.Get(uri);
        }

        public static IWebRequest GetTexture(string uri)
        {
            return Instance.service.GetTexture(uri);
        }

        public static IWebRequest Post(string uri, string postData)
        {
            return Instance.service.Post(uri, postData);
        }

        public static IWebRequest Post(string uri, WWWForm formData)
        {
            return Instance.service.Post(uri, formData);
        }

        public static IWebRequest Post(string uri, Dictionary<string, string> formData)
        {
            return Instance.service.Post(uri, formData);
        }

        public static IWebRequest Post(string uri, List<IMultipartFormSection> multipartForm)
        {
            return Instance.service.Post(uri, multipartForm);
        }

        public static IWebRequest Post(string uri, byte[] bytes, string contentType)
        {
            return Instance.service.Post(uri, bytes, contentType);
        }

        public static IWebRequest PostJson(string uri, string json)
        {
            return Instance.service.PostJson(uri, json);
        }

        public static IWebRequest PostJson<T>(string uri, T payload) where T : class
        {
            return Instance.service.PostJson(uri, payload);
        }

        public static IWebRequest Put(string uri, byte[] bodyData)
        {
            return Instance.service.Put(uri, bodyData);
        }

        public static IWebRequest Put(string uri, string bodyData)
        {
            return Instance.service.Put(uri, bodyData);
        }

        public static IWebRequest Delete(string uri)
        {
            return Instance.service.Delete(uri);
        }

        public static IWebRequest Head(string uri)
        {
            return Instance.service.Head(uri);
        }
        #endregion

        /// <summary>
        /// 发送请求并处理响应
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="onNetworkError"></param>
        internal void Send(IWebRequest request,
            Action<WebResponse> onSuccess = null,
            Action<WebResponse> onError = null,
            Action<WebResponse> onNetworkError = null
         )
        {
            sendRequest = request;
            CoroutinerMgr.Instance.AddRoutine(SendCoroutine(sendRequest, onSuccess, onError, onNetworkError), SendCallback);
        }

        /// <summary>
        /// Send the request and add it to the activity request list
        /// 发送请求并将其添加到活动请求列表中
        /// </summary>
        /// <param name="coroutine"></param>
        private void SendCallback(Coroutine coroutine)
        {
            if (!webRequests.ContainsKey(sendRequest))
            {
                webRequests.Add(sendRequest, coroutine);
            }
        }

        /// <summary>
        /// A coroutine used for sending requests and processing responses
        /// 用于发送请求和处理响应的协程
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="onNetworkError"></param>
        /// <returns></returns>
        private IEnumerator SendCoroutine(IWebRequest request, Action<WebResponse> onSuccess = null,
            Action<WebResponse> onError = null, Action<WebResponse> onNetworkError = null)
        {
            yield return service.Send(request, onSuccess, onError, onNetworkError);
            webRequests.Remove(request);
        }

        /// <summary>
        /// Suspend the request and remove it from the activity request list
        /// 中止请求并将其从活动请求列表中删除
        /// </summary>
        /// <param name="request"></param>
        internal void Abort(IWebRequest request)
        {
            service.Abort(request);

            if (webRequests.TryGetValue(request, out Coroutine coroutine))
            {
                CoroutinerMgr.Instance.StopTargetRoutine(coroutine);
                webRequests.Remove(request);
            }
        }
    }
}
