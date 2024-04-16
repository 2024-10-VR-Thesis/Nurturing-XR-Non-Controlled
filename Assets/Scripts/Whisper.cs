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
using System.Text.RegularExpressions;

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
        private string prompt = "Act as 5 y/o child. Dont break character. Dont ever mention you are an AI model. You are going to create an imaginative question based on an object or a previous conversation, just the question";
        private string scorePrompt = "Score our conversation from 1-10, dont write words just the score based on a child psichology pov";
        private List<int> scores = new List<int>();
        public TextToSpeech tts;
        private bool conversationMode = false; //todo: va en la clase de semaforo
        private bool askAgain = false; //todo: va en la clase de semaforo


        private void Start() {
            #if UNITY_WEBGL && !UNITY_EDITOR
                        dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            /*
            #else
                        foreach (var device in Microphone.devices)
                        {
                            dropdown.options.Add(new Dropdown.OptionData(device));
                        }
                        recordButton.onClick.AddListener(StartRecording);
                        dropdown.onValueChanged.AddListener(ChangeMicrophone);

                        var index = PlayerPrefs.GetInt("user-mic-device-index");
                        dropdown.SetValueWithoutNotify(index);
            #endif
            */
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


        private async Task<int> ExtractRatingFromResponse(string responseText)
        {
            Match match = Regex.Match(responseText, @"\d+");

            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            { 
                return -1 ; //Si no se encuentra un número en el comentario se devuelve -1 por lo que no se imprime en consola
            }
        }

        private async Task GenerateImaginativeQuestion(string transcribedText) //no es necesariamente transcripcion, tambien es objeto
        {
            ChatMessage newMessage = new ChatMessage();
            newMessage.Content = transcribedText;
            newMessage.Role = "user";

            if (messages.Count == 0 || askAgain)
            {
                var fullPrompt = prompt;
                if (askAgain) {
                    messages.Add(transcribedText);
                    var conversationHistory = messages.Skip(Math.Max(0, messages.Length - 2));
                    fullPrompt +=  "Conversation: " + conversationHistory;
                }
                else {
                    var object = transcribedText;
                    fullPrompt += "Object: " + object;
                }
                
                newMessage.Content = fullPrompt;
                //messages.Add(newMessage);

                CreateChatCompletionRequest requestR = new CreateChatCompletionRequest();
                requestR.Messages = messages;
                requestR.Model = "gpt-3.5-turbo";

                CreateChatCompletionRequest requestAI = new CreateChatCompletionRequest();
                requestAI.Messages = messages;
                requestAI.Model = "gpt-3.5-turbo";

                var responseR = await openAI.CreateChatCompletion(requestR);

                var aiResponse = await openAI.CreateChatCompletion(requestAI);

                if (aiResponse.Choices != null && aiResponse.Choices.Count > 0)
                {
                    var chatResponse = aiResponse.Choices[0].Message;
                    string text = chatResponse.Content;
                    //todo: send to poly to speak it
                    messages.Add(text);
                    conversationMode = true
                }
            }

            else if (messages.Count >= 1 && conversationMode)
            {
                
                var answer = transcribedText;
                messages.Add(answer);
                var conversationHistory = messages.Skip(Math.Max(0, messages.Length - 2));
                var fullPrompt =  "Conversation: " + conversationHistory + scorePrompt
                newMessage.Content = fullPrompt;
        
                CreateChatCompletionRequest requestR = new CreateChatCompletionRequest();
                requestR.Messages = messages;
                requestR.Model = "gpt-3.5-turbo";

                var responseR = await openAI.CreateChatCompletion(requestR);

                if (responseR.Choices != null && responseR.Choices.Count > 0)
                {
                    var chatResponse = responseR.Choices[0].Message;
                    string text = chatResponse.Content;
                    Debug.Log(chatResponse.Content);

                    // En este lugar se llama al task para extraer el número e imprimirlo en la consola
                    int rating = await ExtractRatingFromResponse(text);
                    if (rating == -1) {
                        if (scores.Count >= 1) { 
                            rating = score.Average()
                        } 
                        else {
                            rating = 1
                        }
                    }
                    scores.Add(rating)
                    if (scores.Last() >= 7) {
                        conversationMode = false
                        askAgain = false
                        messages.Clear()
                        //todo: add progress to drawing
                    }
                    else {
                        askAgain = true
                        //todo: hacer nueva pregunta
                    }
                    //todo: send score to girl
                    Debug.Log("Calificación obtenida: " + scores.Last());
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
