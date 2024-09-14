using GameLogic;
using GameLogic.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.Examples
{
    public class Example : MonoBehaviour
    {
        public RawImage rawImage;

        void Start()
        {
            Invoke("Test", 2f);
        }

        /// <summary>
        /// Accessing HTTP requires enabling unsafe connections
        /// 访问Http需要开启允许不安全连接
        /// 
        /// Project Settings -> Player -> Allow downloads over HTTP -> Always allowed
        /// </summary>

        public void Test()
        {
            //Get()
            GetTexture("http://gips0.baidu.com/it/u=3602773692,1512483864&fm=3028&app=3028&f=JPEG&fmt=auto?w=960&h=1280");
        }


        public void Get()
        {
            string url = "http://www.baidu.com";

            var request = WebRequestModule.Get(url).
                            OnSuccess((response) =>
                            {
                                Debug.Log("Success");
                                Debug.Log(response.Text);
                            }).
                            OnError((response) =>
                            {
                                Debug.LogError("OnError");
                                Debug.LogError(response.StatusCode);
                            }).
                            Send();
        }

        public void GetTexture(string url)
        {
            var request = WebRequestModule.GetTexture(url).
                            OnSuccess(GetImage).
                            OnDownloadProgress(GetImageProgress).
                            OnError((error) => Debug.LogError(error)).
                            Send();
        }

        public void GetImage(WebResponse httpResponse)
        {
            Texture2D tempTexture = httpResponse.Texture;
            rawImage.texture = tempTexture;
        }

        public void GetImageProgress(float progress)
        {
            Debug.Log("Download progress : " + progress);
        }
    }
}