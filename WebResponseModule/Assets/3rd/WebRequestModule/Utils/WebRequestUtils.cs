using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic.Utils
{
    /// <summary>
    /// Utils
    /// 工具类
    /// </summary>
    public static class WebRequestUtils
    {
        /// <summary>
        /// 构造带有参数的URI
        /// Construct URI with parameters
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ConstructUriWithParameters(string uri, Dictionary<string, string> parameters)
        {
            // 如果参数为空或者数量为0，直接返回原始URI
            // If the parameter is empty or the quantity is 0, return the original URI directly
            if (parameters == null || parameters.Count == 0)
            {
                return uri;
            }

            // 使用StringBuilder来构造新的URI
            // Constructing a new URI using a StringBuilder
            var stringBuilder = new StringBuilder(uri);

            // 使用string.Join方法和LINQ来连接参数
            // Using the string.Join method and Linq to connect parameters
            stringBuilder.Append("?");
            stringBuilder.Append(string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}")));

            // 返回构造好的URI
            // Return the constructed URI
            return stringBuilder.ToString();
        }
    }
}