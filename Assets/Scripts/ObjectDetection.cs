using UnityEngine;
using System.IO;
using System.Net.Http;
using System.Collections;
using TMPro;
using UnityEngine.Networking;

public class ObjectDetection : MonoBehaviour
{
    public TMP_Text worldText;

    void Start()
    {
        // FIXME: each headset should define when to start, in separate file
        StartCoroutine(DelayedCaptureAndSendScreenshot());
    }

    IEnumerator DelayedCaptureAndSendScreenshot()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // FIXME: this should be in Start()
        StartCoroutine(CaptureAndSendScreenshot());
    }

    IEnumerator CaptureAndSendScreenshot()
    {
        string screenshotName = System.Guid.NewGuid().ToString();
        string screenshotPath = Path.Combine(Application.dataPath, "ScreenCaptures", screenshotName);
        string normalizedPath = screenshotPath.Replace("\\", "/");

        string url = "http://127.0.0.1:5000/screenshot";
        Debug.Log("This is the path: " + screenshotPath);
        string jsonBody = "{\"screenshot_name\": \"" + normalizedPath + "\"}";
        print(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        /*
        else
        {
            Debug.Log($"Screenshot request with name {normalizedPath} sent successfully");
        }
        */

        SendScreenshot(screenshotPath+".png");
    }

    async void SendScreenshot(string screenshotPath)
    {
        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiZjg4ZWMyNDUtOGNhMi00MGE2LWE4NzEtMWM5ZmU0ZmE5YWNlIiwidHlwZSI6ImFwaV90b2tlbiJ9.BV9MP-DFCnUdMlrkkN9E-nWR_ThUmTlI11-eBlguDRM";
        string url = "https://api.edenai.run/v2/image/object_detection";

        MultipartFormDataContent form = new MultipartFormDataContent();
        form.Add(new StringContent("amazon"), "providers");
        form.Add(new StringContent("google"), "fallback_providers");

        byte[] imageBytes = File.ReadAllBytes(screenshotPath);
        ByteArrayContent imageContent = new ByteArrayContent(imageBytes);
        form.Add(imageContent, "file", Path.GetFileName(screenshotPath));

        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        HttpResponseMessage response = await client.PostAsync(url, form);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

            Debug.Log(result.amazon.items);
            worldText.text = string.Join(", ", result.amazon.items);
        }
        else
        {
            Debug.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase} - {response}");
            //worldText.text = $"Error: {response.StatusCode} - {response.ReasonPhrase} - {response}";
        }

        // TODO: delete screenshots, but wait until game is finished, otherwise this wont load

    }
}
