using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : MonoBehaviour
{
    public string serverIP = string.Empty; // your Windows PC IP
    public int port = 8080;

    public IEnumerator SendPNG(byte[] tex)
    {
        serverIP = PlayerPrefs.GetString("ip");

        byte[] pngBytes = tex;

        string url = $"http://{serverIP}:{port}/upload";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(pngBytes);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "image/png");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Upload failed: " + req.error);
        }
        else
        {
            Debug.Log("Upload success");
        }
    }
}
