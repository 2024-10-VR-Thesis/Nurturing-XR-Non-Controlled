using UnityEngine;
using System.IO;
using System.Net.Http;
using System.Collections;

public class ObjectDetection : MonoBehaviour
{

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
        string screenshotName = System.Guid.NewGuid().ToString() + ".png";
        string screenshotPath = Path.Combine(Application.dataPath, "ScreenCaptures", screenshotName);

        ScreenCapture.CaptureScreenshot(screenshotPath);
        Debug.Log("Screenshot captured and saved at: " + screenshotPath);

        // Wait until the screenshot file exists
        while (!File.Exists(screenshotPath))
        {
            yield return null;
        }

        SendScreenshot(screenshotPath);
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
        }
        else
        {
            Debug.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase} - {response}");
        }

        // TODO: delete screenshots, but wait until game is finished, otherwise this wont load

    }
}
