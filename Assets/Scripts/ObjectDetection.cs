using UnityEngine;
using System.IO;
using System.Net.Http;
using System.Collections;
using TMPro;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class ObjectDetection : MonoBehaviour
{
    public TMP_Text worldText;

    void Start()
    {
        // FIXME: each headset should define when to start, in separate file
        //StartCoroutine(DetectObjects());
    }

    IEnumerator DetectObjects()
    {
        string url = "http://127.0.0.1:5000/detect";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(null);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            List<string> responseList = JsonConvert.DeserializeObject<List<string>>(jsonResponse);
            worldText.text = responseList[2];
        }

    }

}
