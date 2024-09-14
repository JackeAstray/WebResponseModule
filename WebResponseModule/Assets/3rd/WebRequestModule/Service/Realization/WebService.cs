using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using LitJson;

namespace GameLogic.Service
{
    public class WebService : IWebService
    {

        public IWebRequest Get(string uri)
        {
            return new WebRequest(UnityWebRequest.Get(uri));
        }

        public IWebRequest GetTexture(string uri)
        {
            return new WebRequest(UnityWebRequestTexture.GetTexture(uri));
        }

        public IWebRequest Post(string uri, string postData)
        {
            return new WebRequest(UnityWebRequest.PostWwwForm(uri, postData));
        }

        public IWebRequest Post(string uri, WWWForm formData)
        {
            return new WebRequest(UnityWebRequest.Post(uri, formData));
        }

        public IWebRequest Post(string uri, Dictionary<string, string> formData)
        {
            return new WebRequest(UnityWebRequest.Post(uri, formData));
        }

        public IWebRequest Post(string uri, List<IMultipartFormSection> multipartForm)
        {
            return new WebRequest(UnityWebRequest.Post(uri, multipartForm));
        }

        public IWebRequest Post(string uri, byte[] bytes, string contentType)
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST);
            unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", contentType);
            return new WebRequest(unityWebRequest);
        }

        public IWebRequest PostJson(string uri, string json)
        {
            return Post(uri, Encoding.UTF8.GetBytes(json), "application/json");
        }

        public IWebRequest PostJson<T>(string uri, T payload) where T : class
        {
            var json = JsonMapper.ToJson(payload);
            return PostJson(uri, json);
        }

        public IWebRequest Put(string uri, byte[] bodyData)
        {
            return new WebRequest(UnityWebRequest.Put(uri, bodyData));
        }

        public IWebRequest Put(string uri, string bodyData)
        {
            return new WebRequest(UnityWebRequest.Put(uri, bodyData));
        }

        public IWebRequest Delete(string uri)
        {
            return new WebRequest(UnityWebRequest.Delete(uri));
        }

        public IWebRequest Head(string uri)
        {
            return new WebRequest(UnityWebRequest.Head(uri));
        }

        public IEnumerator Send(IWebRequest request, Action<WebResponse> onSuccess = null,
            Action<WebResponse> onError = null, Action<WebResponse> onNetworkError = null)
        {
            var unityHttpRequest = (WebRequest)request;
            using (UnityWebRequest unityWebRequest = unityHttpRequest.UnityWebRequest)
            {
                yield return unityWebRequest.SendWebRequest();

                var response = new WebResponse
                {
                    Url = unityWebRequest.url,
                    Bytes = unityWebRequest.downloadHandler?.data,
                    Text = unityWebRequest.downloadHandler?.text,
                    IsSuccessful = unityWebRequest.result != UnityWebRequest.Result.ConnectionError && unityWebRequest.result != UnityWebRequest.Result.ProtocolError,
                    IsHttpError = unityWebRequest.result == UnityWebRequest.Result.ProtocolError,
                    IsNetworkError = unityWebRequest.result == UnityWebRequest.Result.ConnectionError,
                    Error = unityWebRequest.error,
                    StatusCode = unityWebRequest.responseCode,
                    ResponseHeaders = unityWebRequest.GetResponseHeaders(),
                    Texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture)?.texture
                };

                if (response.IsNetworkError) // 使用修正后的条件
                {
                    onNetworkError?.Invoke(response);
                }
                else if (response.IsHttpError) // 使用修正后的条件
                {
                    onError?.Invoke(response);
                }
                else if (response.IsSuccessful) // 检查请求是否成功
                {
                    onSuccess?.Invoke(response);
                }
            }
        }

        public void Abort(IWebRequest request)
        {
            var unityHttpRequest = request as WebRequest;
            if (unityHttpRequest?.UnityWebRequest != null && !unityHttpRequest.UnityWebRequest.isDone)
            {
                unityHttpRequest.UnityWebRequest.Abort();
            }
        }
    }
}
