using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace GameLogic.Service
{
    /// <summary>
    /// Web Request
    /// </summary>
    public class WebRequest : IWebRequest, IUpdateProgress
    {
        internal UnityWebRequest UnityWebRequest => unityWebRequest;

        private readonly UnityWebRequest unityWebRequest;
        private readonly Dictionary<string, string> headers;

        private event Action<float> onUploadProgress;
        private event Action<float> onDownloadProgress;
        private event Action<WebResponse> onSuccess;
        private event Action<WebResponse> onError;
        private event Action<WebResponse> onNetworkError;

        private float downloadProgress;
        private float uploadProgress;

        public WebRequest(UnityWebRequest unityWebRequest)
        {
            this.unityWebRequest = unityWebRequest;
            headers = new Dictionary<string, string>(WebRequestModule.Instance.GetSuperHeaders());
        }

        public IWebRequest RemoveSuperHeaders()
        {
            foreach (var kvp in WebRequestModule.Instance.GetSuperHeaders())
            {
                headers.Remove(kvp.Key);
            }

            return this;
        }

        public IWebRequest SetHeader(string key, string value)
        {
            headers[key] = value;
            return this;
        }

        public IWebRequest SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var kvp in headers)
            {
                SetHeader(kvp.Key, kvp.Value);
            }

            return this;
        }

        public IWebRequest OnUploadProgress(Action<float> onProgress)
        {
            onUploadProgress += onProgress;
            return this;
        }

        public IWebRequest OnDownloadProgress(Action<float> onProgress)
        {
            onDownloadProgress += onProgress;
            return this;
        }

        public IWebRequest OnSuccess(Action<WebResponse> onSuccess)
        {
            this.onSuccess += onSuccess;
            return this;
        }

        public IWebRequest OnError(Action<WebResponse> onError)
        {
            this.onError += onError;
            return this;
        }

        public IWebRequest OnNetworkError(Action<WebResponse> onNetworkError)
        {
            this.onNetworkError += onNetworkError;
            return this;
        }

        public bool RemoveHeader(string key)
        {
            return headers.Remove(key);
        }

        public IWebRequest SetTimeout(int duration)
        {
            unityWebRequest.timeout = duration;
            return this;
        }

        public IWebRequest Send()
        {
            foreach (var header in headers)
            {
                unityWebRequest.SetRequestHeader(header.Key, header.Value);
            }

            WebRequestModule.Instance.Send(this, onSuccess, onError, onNetworkError);
            return this;
        }

        public IWebRequest SetRedirectLimit(int redirectLimit)
        {
            UnityWebRequest.redirectLimit = redirectLimit;
            return this;
        }

        public void UpdateProgress()
        {
            UpdateProgress(ref downloadProgress, unityWebRequest.downloadProgress, onDownloadProgress);
            UpdateProgress(ref uploadProgress, unityWebRequest.uploadProgress, onUploadProgress);
        }

        public void Abort()
        {
            WebRequestModule.Instance.Abort(this);
        }

        private void UpdateProgress(ref float currentProgress, float progress, Action<float> onProgress)
        {
            if (currentProgress < progress)
            {
                currentProgress = progress;
                onProgress?.Invoke(currentProgress);
            }
        }
    }
}
