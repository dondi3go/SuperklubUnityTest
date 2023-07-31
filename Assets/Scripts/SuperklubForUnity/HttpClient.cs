using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Superklub;

/// <summary>
/// An implementation of interface Superklub.IHttpClient for Unity
/// </summary>
public class HttpClient : IHttpClient
{
    private bool verbose = false;
    private string url = "http://127.0.0.1:9999/api/channels/default";

    /// <summary>
    /// 
    /// </summary>
    public async Task<HttpResponse> PostAsync(string jsonString)
    {
        UnityWebRequest req = WebRequest.Post(url, jsonString);
        if (!await WebRequest.FullProcess(req, verbose))
        {
            return new HttpResponse((int)req.responseCode, req.error);
        }

        return new HttpResponse((int)req.responseCode, req.downloadHandler.text);
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task<HttpResponse> GetAsync()
    {
        UnityWebRequest req = WebRequest.Get(url);
        if (!await WebRequest.FullProcess(req, verbose))
        {
            return new HttpResponse((int)req.responseCode, req.error);
        }

        return new HttpResponse((int)req.responseCode, req.downloadHandler.text);
    }
}
