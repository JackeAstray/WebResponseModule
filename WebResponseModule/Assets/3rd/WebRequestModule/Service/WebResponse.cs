﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic.Service
{
    /// <summary>
    /// Web Response
    /// 网络响应
    /// </summary>
    public class WebResponse
    {
        public string Url { get; set; }
        public bool IsSuccessful { get; set; }
        public bool IsHttpError { get; set; }
        public bool IsNetworkError { get; set; }
        public long StatusCode { get; set; }
        public byte[] Bytes { get; set; }
        public string Text { get; set; }
        public string Error { get; set; }
        public Texture2D Texture { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; set; }
    }
}
