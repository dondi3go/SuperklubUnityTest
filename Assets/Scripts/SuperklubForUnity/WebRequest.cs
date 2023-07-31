using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// A certificate handler alway OK whatever the certificate data
/// Usefull to access some HTTPS APIs
/// WARNING : THIS IS PROBABLY NOT THE BEST APPROACH ON A SECURITY POINT OF VIEW
/// </summary>
class CustomCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}


/// <summary>
/// Encapsulate operations on UnityWebRequest objects
/// 
/// Create CRUD UnityWebRequest objects
/// compatible with Flask
/// 
/// Errors are always logged
/// Request time is displayed when "verbose" is true
/// </summary>
public class WebRequest
{
    private const string POST = "POST";
    private const string PUT = "PUT";
    private const string DELETE = "DELETE";

    private const string CONTENT_TYPE = "Content-Type";
    private const string APPLICATION_JSON = "application/json";

    private const long HTTP_CODE_204 = 204; // No content
    private const long HTTP_CODE_400 = 400; // Bad request
    private const long HTTP_CODE_401 = 401; // Unauthorized
    private const long HTTP_CODE_404 = 404; // Not found
    private const long HTTP_CODE_422 = 422; // Unprocessable Entity

    /// <summary>
    /// Create a POST request with a json string as a parameter
    /// </summary>
    public static UnityWebRequest Post(string url, string json)
    {
        UnityWebRequest www = new UnityWebRequest(url, POST);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader(CONTENT_TYPE, APPLICATION_JSON);
        www.certificateHandler = new CustomCertificateHandler();

        return www;
    }

    /// <summary>
    /// Create a GET request
    /// </summary>
    public static UnityWebRequest Get(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.certificateHandler = new CustomCertificateHandler();
        return www;
    }

    /// <summary>
    /// Create a PUT request
    /// </summary>
    public static UnityWebRequest Put(string url, string json)
    {
        UnityWebRequest www = new UnityWebRequest(url, PUT);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader(CONTENT_TYPE, APPLICATION_JSON);
        www.certificateHandler = new CustomCertificateHandler();

        return www;
    }

    /// <summary>
    /// Create a DELETE request
    /// </summary>
    public static UnityWebRequest Delete(string url)
    {
        UnityWebRequest www = UnityWebRequest.Delete(url);
        www.certificateHandler = new CustomCertificateHandler();
        return www;
    }


    /// <summary>
    /// Send web request, log parameters and time
    /// </summary>
    public static async Task Send(UnityWebRequest www, bool verbose = false)
    {
        if (verbose)
        {
            Log("Sending request, url = " + www.url + " [" + www.method + "]");
        }
        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
        await www.SendWebRequest();
        sw.Stop();
        if (verbose)
        {
            string logMessage = "Receiving code " + www.responseCode + " (" + sw.Elapsed.TotalSeconds + " s)";
            if (IsHTTPCodeAnError(www.responseCode))
            {
                LogError(logMessage);
            }
            else
            {
                Log(logMessage);
            }
            if (www.downloadHandler != null)
            {
                if (!string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    string textResponse = www.downloadHandler.text.TrimEnd('\n');
                    Log("Received content = " + textResponse);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static bool IsHTTPCodeAnError(long HTTPCode)
    {
        if (HTTPCode == HTTP_CODE_204 ||
            HTTPCode == HTTP_CODE_400 ||
            HTTPCode == HTTP_CODE_401 ||
            HTTPCode == HTTP_CODE_404 ||
            HTTPCode == HTTP_CODE_422)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check request errors 
    /// Call this after www.SendWebRequest()
    /// </summary>
    public static bool IsError(UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.ProtocolError ||
            www.result == UnityWebRequest.Result.ConnectionError)
        {
            LogError("Error in " + www.url + " [" + www.method + "] : " + www.error);
            return true;
        }

        if (IsHTTPCodeAnError(www.responseCode))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check JSON string result (relevant for POST, GET and PUT, but not DELETE)
    /// Call this after www.SendWebRequest()
    /// </summary>
    public static bool IsJSONResultInvalid(UnityWebRequest www)
    {
        string textResponse = www.downloadHandler.text.TrimEnd('\n');

        if (string.IsNullOrEmpty(textResponse))
        {
            LogError("Error in " + www.url + " [" + www.method + "] :");
            LogError("Null or empty json result");
            return true;
        }

        // Very basic checks, not a full review

        // Json array
        if (textResponse.StartsWith("[") && textResponse.EndsWith("]"))
        {
            return false;
        }

        // Json object
        if (textResponse.StartsWith("{") && textResponse.EndsWith("}"))
        {
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Send the request + Check for errors, in one single call
    /// Will return false if the result is not JSON element or JSON array (except for DELETE where a bool is expected)
    /// </summary>
    public static async Task<bool> FullProcess(UnityWebRequest www, bool verbose = false)
    {
        await Send(www, verbose);

        if (IsError(www))
        {
            return false;
        }

        if (www.method != DELETE)
        {
            if (IsJSONResultInvalid(www))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Log
    /// </summary>
    private static void LogError(string message)
    {
        Debug.LogError("[WebRequest] " + message);
    }
    private static void LogWarning(string message)
    {
        Debug.LogWarning("[WebRequest] " + message);
    }
    private static void Log(string message)
    {
        Debug.Log("[WebRequest] " + message);
    }
}
