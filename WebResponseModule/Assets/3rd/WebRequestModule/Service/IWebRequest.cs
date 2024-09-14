using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Service
{
    /// <summary>
    /// Web Request (Http and Https)
    /// 网络请求 (Http and Https)
    /// </summary>
    public interface IWebRequest
    {
        #region Header
        IWebRequest RemoveSuperHeaders();
        bool RemoveHeader(string key);
        IWebRequest SetHeader(string key, string value);
        IWebRequest SetHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        #endregion

        #region Progress
        /// <summary>
        /// On upload progress
        /// 上传进度
        /// </summary>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        IWebRequest OnUploadProgress(Action<float> onProgress);
        /// <summary>
        /// On download progress
        /// 下载进度
        /// </summary>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        IWebRequest OnDownloadProgress(Action<float> onProgress);
        #endregion

        #region Callback
        /// <summary>
        /// On success
        /// 成功
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        IWebRequest OnSuccess(Action<WebResponse> onSuccess);
        /// <summary>
        /// On error
        /// 错误
        /// </summary>
        /// <param name="onError"></param>
        /// <returns></returns>
        IWebRequest OnError(Action<WebResponse> onError);
        /// <summary>
        /// On network error
        /// 网络错误
        /// </summary>
        /// <param name="onNetworkError"></param>
        /// <returns></returns>
        IWebRequest OnNetworkError(Action<WebResponse> onNetworkError);
        #endregion

        /// <summary>
        /// Set timeout
        /// 设置超时
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        IWebRequest SetTimeout(int duration);
        /// <summary>
        /// Send request
        /// 发送请求
        /// </summary>
        /// <returns></returns>
        IWebRequest Send();
        /// <summary>
        /// Set redirect limit
        /// 设置重定向限制
        /// </summary>
        /// <param name="redirectLimit"></param>
        /// <returns></returns>
        IWebRequest SetRedirectLimit(int redirectLimit);

        void Abort();
    }
}
