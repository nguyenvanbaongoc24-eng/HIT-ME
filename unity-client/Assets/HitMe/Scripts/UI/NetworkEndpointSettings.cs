using System;
using UnityEngine;
namespace HitMe.UI
{
    public static class NetworkEndpointSettings
    {
        public const string ProductionEndpoint = "wss://hit-me-zj17.onrender.com/play";
        public static bool IsLocal(string url)=>Uri.TryCreate(url,UriKind.Absolute,out var uri)&&uri.IsLoopback;
        public static string Current
        {
            get
            {
                var saved=PlayerPrefs.GetString("NetworkEndpoint","");
                bool development=Application.isEditor||IsLocal(Application.absoluteURL);
                if(saved.Length==0)return development?"ws://127.0.0.1:8788/play":ProductionEndpoint;
                return development||Uri.TryCreate(saved,UriKind.Absolute,out var uri)&&uri.Scheme=="wss"&&!uri.IsLoopback?saved:"";
            }
        }
        public static bool IsAllowed(string url)=>Uri.TryCreate(url,UriKind.Absolute,out var uri)&&
            (uri.Scheme=="wss"&&(!uri.IsLoopback||Application.isEditor||IsLocal(Application.absoluteURL))||uri.Scheme=="ws"&&(Application.isEditor||IsLocal(Application.absoluteURL))&&uri.IsLoopback);
    }
}
