using Amazon.Polly.Model;
using Amazon.Polly;
using Amazon.Runtime;
using OpenAI;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Scripts.TexToSpeech;
using Amazon;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();
        private OpenAIApi openAI = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as 3 year old child. Dont break character. Don't ever mentioned you are an AI model. We are going to play a game where i mentioned an object and you have to make 1 imaginative question about the question as 3 year old kid would do";
        private string scorePrompt = "Score our conversation from 1-10, dont write words just the score based on a psichology pov";
        private List<string> scores;
        public TextToSpeech tts;

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
            /*
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            scores = new List<string>();
            */

            GenerateImaginativeQuestion("fork");
            GenerateImaginativeQuestion("fork");
#endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }

        private void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");

#if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
#endif
        }

        private async void EndRecording()
        {
            message.text = "Transcripting...";

#if !UNITY_WEBGL
            Microphone.End(null);
#endif

            byte[] data = SaveWav.Save(fileName, clip);

            // Obtener la transcripción del audio
            string transcribedText = await GetAudioTranscription(data);


            // Enviar la transcripción a ChatGPT para obtener la pregunta imaginativa
            Debug.Log(transcribedText);
            await GenerateImaginativeQuestion(transcribedText);


            // Restablecer la UI
            progressBar.fillAmount = 0;
            recordButton.enabled = true;
        }

        private async Task<string> GetAudioTranscription(byte[] audioData)
        {
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = audioData, Name = "audio.wav" },
                Model = "whisper-1",
                Language = "en"
            };

            var res = await openai.CreateAudioTranscription(req);
            return res.Text;
        }

        private async Task GenerateImaginativeQuestion(string transcribedText)
        {
            ChatMessage newMessage = new ChatMessage();
            newMessage.Content = transcribedText;
            newMessage.Role = "user";

            if (messages.Count == 0)
            {
                newMessage.Content = prompt;
                messages.Add(newMessage);

                CreateChatCompletionRequest requestR = new CreateChatCompletionRequest();
                requestR.Messages = messages;
                requestR.Model = "gpt-3.5-turbo";

                var responseR = await openAI.CreateChatCompletion(requestR);

                if (responseR.Choices != null && responseR.Choices.Count > 0)
                {
                    var chatResponse = responseR.Choices[0].Message;
                    messages.Add(chatResponse);

                    Debug.Log(chatResponse.Content);
                }

            }

            else if (messages.Count % 10 == 0)
            {
                newMessage.Content = scorePrompt;
                messages.Add(newMessage);

                CreateChatCompletionRequest requestR = new CreateChatCompletionRequest();
                requestR.Messages = messages;
                requestR.Model = "gpt-3.5-turbo";

                var responseR = await openAI.CreateChatCompletion(requestR);

                if (responseR.Choices != null && responseR.Choices.Count > 0)
                {
                    var chatResponse = responseR.Choices[0].Message;
                    messages.Add(chatResponse);

                    Debug.Log(chatResponse.Content);
                }

            }

            messages.Add(newMessage);

            CreateChatCompletionRequest request = new CreateChatCompletionRequest();
            request.Messages = messages;
            request.Model = "gpt-3.5-turbo";

            var response = await openAI.CreateChatCompletion(request);

            if (response.Choices != null && response.Choices.Count > 0)
            {
                var chatResponse = response.Choices[0].Message;

                string text = chatResponse.Content;

                Debug.Log("este es el texto " + text);



                messages.Add(chatResponse);

                Debug.Log(chatResponse.Content);
            }
            int index = messages.Count - 1;
            tts.setSpeak(messages[index].Content);
            tts.texttospeech();

        }


        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}