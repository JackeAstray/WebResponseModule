using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic.Utils
{
    /// <summary>
    /// Utils
    /// ������
    /// </summary>
    public static class WebRequestUtils
    {
        /// <summary>
        /// ������в�����URI
        /// Construct URI with parameters
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ConstructUriWithParameters(string uri, Dictionary<string, string> parameters)
        {
            // �������Ϊ�ջ�������Ϊ0��ֱ�ӷ���ԭʼURI
            // If the parameter is empty or the quantity is 0, return the original URI directly
            if (parameters == null || parameters.Count == 0)
            {
                return uri;
            }

            // ʹ��StringBuilder�������µ�URI
            // Constructing a new URI using a StringBuilder
            var stringBuilder = new StringBuilder(uri);

            // ʹ��string.Join������LINQ�����Ӳ���
            // Using the string.Join method and Linq to connect parameters
            stringBuilder.Append("?");
            stringBuilder.Append(string.Join("&", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}")));

            // ���ع���õ�URI
            // Return the constructed URI
            return stringBuilder.ToString();
        }
    }
}