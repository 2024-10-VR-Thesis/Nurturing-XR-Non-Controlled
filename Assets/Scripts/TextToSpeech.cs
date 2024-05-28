using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

namespace Scripts.TexToSpeech
{
    public class TextToSpeech : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        public string introSpeak;
        public Conversation.Conversation conversation;
        public AnimationsHandler animationsHandler;
        public ClockHandler clockHandler;
        public ConversationStarter conversationStarter;

        async void Start()
        {
            string path = "Assets/Static speech/tutorial.json";
            string jsonString = File.ReadAllText(path);
            TextToSpeechData data = JsonUtility.FromJson<TextToSpeechData>(jsonString);
            await Task.Delay(17000);
            introSpeak = data.EN;
            texttospeech(introSpeak, true);
            //introSpeak = null;
        }

        void Update()
        {
        }

        public void setSpeak(string text)
        {
            introSpeak = text;
        }

        public async void texttospeech(string speak, bool tutorial = false)
        {
            //conversation.talking = true;
            string speechCopy = speak;
            speak = null;
            var request = new SynthesizeSpeechRequest()
            {
                Text = speechCopy,
                Engine = Engine.Neural,
                VoiceId = VoiceId.Ivy,
                OutputFormat = OutputFormat.Mp3
            };
            var credentials = new BasicAWSCredentials("AKIAZQ3DSEZUN3WECAN6", "HYQk/wKoN2Q40nwrmBvhXWTnv+cO7BBobeRaFwGk");
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

                await Task.Delay((int)(clip.length * 1000)); // Convert clip length from seconds to milliseconds
                conversation.talking = false;
                conversation.listening = true;
            }

            if (tutorial)
            {
                StartCoroutine(clockHandler.StartTime());
                StartCoroutine(conversationStarter.StartConversation());
            }
        }

        private void WriteintoFile(Stream stream)
        {
            using (var filestream = new FileStream($"{Application.persistentDataPath}/audio.mp3", FileMode.Create))
            {
                byte[] buffer = new byte[8 * 1024];
                int bytesRead = 0;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
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
        public string ES;
    }

}
