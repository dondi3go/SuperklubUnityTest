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

    /// <summary>
    /// 
    /// </summary>
    public async Task<HttpResponse> PostAsync(string url, string jsonString)
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
    public async Task<HttpResponse> GetAsync(string url)
    {
        UnityWebRequest req = WebRequest.Get(url);
        if (!await WebRequest.FullProcess(req, verbose))
        {
            return new HttpResponse((int)req.responseCode, req.error);
        }
        return new HttpResponse((int)req.responseCode, req.downloadHandler.text);
    }
}
