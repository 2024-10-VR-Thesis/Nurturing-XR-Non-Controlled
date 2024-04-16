using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

public class TextToSpeech : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public string speak;
    // Start is called before the first frame update
    void Start()
    {
       speak = "hello, im Ivi";
        texttospeech();
    }

    void Update()
    {
    }

    public void setSpeak(string text)
    {
        Debug.Log("Entro");
        speak = text;
    }

    public async void texttospeech( )
    {
        Debug.Log("dice");
        var request = new SynthesizeSpeechRequest()
        {
            Text = speak,
            Engine = Engine.Neural,
            VoiceId = VoiceId.Ivy,
            OutputFormat = OutputFormat.Mp3
        };
        var credentials = new BasicAWSCredentials("","");
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
        }
        speak = "";
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
