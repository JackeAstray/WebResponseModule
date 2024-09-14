using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace GameLogic.Service
{
    /// <summary>
    /// Web Service (Http and Https)
    /// 网络服务 (Http and Https)
    /// </summary>
    public interface IWebService
    {
        IWebRequest Get(string uri);
        IWebRequest GetTexture(string uri);
        IWebRequest Post(string uri, string postData);
        IWebRequest Post(string uri, WWWForm formData);
        IWebRequest Post(string uri, Dictionary<string, string> formData);
        IWebRequest Post(string uri, List<IMultipartFormSection> multipartForm);
        IWebRequest Post(string uri, byte[] bytes, string contentType);
        IWebRequest PostJson(string uri, string json);
        IWebRequest PostJson<T>(string uri, T payload) where T : class;
        IWebRequest Put(string uri, byte[] bodyData);
        IWebRequest Put(string uri, string bodyData);
        IWebRequest Delete(string uri);
        IWebRequest Head(string uri);
        IEnumerator Send(IWebRequest request, Action<WebResponse> onSuccess = null, Action<WebResponse> onError = null, Action<WebResponse> onNetworkError = null);

        void Abort(IWebRequest request);
    }
}
