using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using Scripts.Conversation;

namespace Scripts.TexToSpeech
{
    public class TextToSpeech : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public string introSpeak;
    public Conversation.Conversation Conversation;

    void Start()
    {
        string path = "Assets/Static speech/tutorial.json";
        string jsonString = File.ReadAllText(path);
        TextToSpeechData data = JsonUtility.FromJson<TextToSpeechData>(jsonString);
        introSpeak = data.EN;
        texttospeech(introSpeak);
        //introSpeak = null;
    }

    void Update()
    {
    }

    public void setSpeak(string text)
    {
        Debug.Log("Entro");
        introSpeak = text;
    }

    public async void texttospeech(string speak)
    {
        string speechCopy = speak;
        speak = null;
        Debug.Log("dice");
        var request = new SynthesizeSpeechRequest()
        {
            Text = speechCopy,
            Engine = Engine.Neural,
            VoiceId = VoiceId.Ivy,
            OutputFormat = OutputFormat.Mp3
        };
        var credentials = new BasicAWSCredentials("", "");
        var client = new AmazonPollyClient(credentials, RegionEndpoint.USEast1);
        var response = await client.SynthesizeSpeechAsync(request);
        WriteintoFile(response.AudioStream);
        using (var www = UnityWebRequestMultimedia.GetAudioClip($"{Application.persistentDataPath}/audio.mp3", AudioType.MPEG))
        {
            var op = www.SendWebRequest();
            while (!op.isDone) await Task.Yield();
            var clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.clip = clip;
            audioSource.Play();

            Conversation.talking = true;
            await Task.Delay((int)(clip.length * 1000)); // Convert clip length from seconds to milliseconds
            Conversation.talking = false;
            }
    }

    private void WriteintoFile(Stream stream)
    {
        using (var filestream = new FileStream($"{Application.persistentDataPath}/audio.mp3", FileMode.Create))
        {
            byte[] buffer = new byte[8*1024];
            int bytesRead = 0;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) >0)
            {
                filestream.Write(buffer, 0, bytesRead);
            }
        }
    }


}
    [System.Serializable]
    public class TextToSpeechData
    {
        public string EN; // "EN" is the key for English text
    }

}
