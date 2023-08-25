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
    public async Task<HttpResponse> PostAsync(
        string url, 
        string jsonString,
        List<CustomRequestHeader>? customRequestHeaders = null,
        CustomResponseHeader? customResponseHeader = null)
    {
        UnityWebRequest req = WebRequest.Post(url, jsonString);
        AddRequestHeaders(req, customRequestHeaders);
        if (!await WebRequest.FullProcess(req, verbose))
        {
            return new HttpResponse((int)req.responseCode, req.error);
        }
        string customHeaderValue = GetResponseHeaderValue(req, customResponseHeader);
        return new HttpResponse((int)req.responseCode, req.downloadHandler.text, customHeaderValue);
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task<HttpResponse> GetAsync(
        string url,
        List<CustomRequestHeader>? customRequestHeaders = null,
        CustomResponseHeader? customResponseHeader = null)
    {
        UnityWebRequest req = WebRequest.Get(url);
        AddRequestHeaders(req, customRequestHeaders);
        if (!await WebRequest.FullProcess(req, verbose))
        {
            return new HttpResponse((int)req.responseCode, req.error);
        }
        string customHeaderValue = GetResponseHeaderValue(req, customResponseHeader);
        return new HttpResponse((int)req.responseCode, req.downloadHandler.text, customHeaderValue);
    }

    /// <summary>
    /// Add header (key+value) to request
    /// </summary>
    private void AddRequestHeaders(UnityWebRequest req, List<CustomRequestHeader>? customRequestHeaders)
    {
        if(customRequestHeaders == null)
        {
            return;
        }
        foreach(CustomRequestHeader header in customRequestHeaders)
        {
            req.SetRequestHeader(header.Key, header.Value);
        }
    }

    /// <summary>
    /// Extract header value from response
    /// </summary>
    private string GetResponseHeaderValue(UnityWebRequest req, CustomResponseHeader? customResponseHeader)
    {
        if(customResponseHeader == null)
        {
            return string.Empty;
        }
        return req.GetResponseHeader(customResponseHeader.Key);
    }
}
