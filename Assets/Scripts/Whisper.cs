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
using System.Linq;
using System;
using Unity.VisualScripting;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Text message;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();
        private OpenAIApi openAI = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as 5 y/o child. Dont break character. Dont ever mention you are an AI model. You are going to create an imaginative question based on an object or a previous answer, just the question. ";
        private string scorePrompt = "Score the answer given to this question from 1-10, dont write words just the score based on a 5 y/o child pov and if the answer and question are in the same context. ";
        private List<int> scores = new List<int>();
        public TextToSpeech tts;
        private bool conversationMode = false; //todo: va en la clase de semaforo
        private bool askAgain = false; //todo: va en la clase de semaforo
        private CreateChatCompletionRequest requestAI;
        private String question;


        private void Start() {

            //GenerateImaginativeQuestion("Pillow");

            Debug.Log("Inicio");
        }

        private void StartRecording()
        {
            isRecording = true;

            #if !UNITY_WEBGL
                        clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
            #endif
        }

        private async void EndRecording()
        {

            #if !UNITY_WEBGL
                        Microphone.End(null);
            #endif

            byte[] data = SaveWav.Save(fileName, clip);

            // Obtener la transcripción del audio
            string transcribedText = await GetAudioTranscription(data);


            // Enviar la transcripción a ChatGPT para obtener la pregunta imaginativa
            await GenerateImaginativeQuestion(transcribedText);


            // Restablecer la UI
            //progressBar.fillAmount = 0;
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
            Debug.Log(res.Text);
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
            //newMessage.Content = transcribedText;
            newMessage.Role = "user";
            if (messages.Count == 0 || askAgain)
            {
                var fullPrompt = prompt;
                if (askAgain) {
                    var answer = transcribedText;
                    fullPrompt += "Previous answer: " + answer;
                }
                else {
                    var objeto = transcribedText;
                    fullPrompt += "Object: " + objeto;
                }

                newMessage.Content = fullPrompt;
                messages.Add(newMessage);
                Debug.Log(messages);

                requestAI = new CreateChatCompletionRequest();
                requestAI.Messages = messages;
                requestAI.Model = "gpt-3.5-turbo";

                var aiResponse = await openAI.CreateChatCompletion(requestAI);

                if (aiResponse.Choices != null && aiResponse.Choices.Count > 0)
                {
                    var chatResponse = aiResponse.Choices[0].Message;
                    messages.Add(chatResponse);
                    string text = chatResponse.Content;
                    print(text);
                    question = text;
                    tts.texttospeech(text);
                    conversationMode = true;              
                }
            }

            if (messages.Count >= 1 && conversationMode)
            {
                var fullPrompt = scorePrompt;
                var answer = "You can not fly";
                fullPrompt += "Question: " + question + ".Answer: " + answer;
                newMessage.Content = fullPrompt;

                messages.Add(newMessage);
        
                var responseR = await openAI.CreateChatCompletion(requestAI);

                if (responseR.Choices != null && responseR.Choices.Count > 0)
                {
                    var chatResponse = responseR.Choices[0].Message;
                    string text = chatResponse.Content;
                    Debug.Log(chatResponse.Content);

                    // En este lugar se llama al task para extraer el número e imprimirlo en la consola
                    int rating = await ExtractRatingFromResponse(text);
                    if (rating == -1) {
                        if (scores.Count >= 1) {
                            List<double> provisional = scores.Select(x => (double)x).ToList();
                            rating = (int)Math.Floor(provisional.Average());
                        } 
                        else {
                            rating = 1;
                        }
                    }
                    scores.Add(rating);
                    if (scores.Last() >= 7) {
                        conversationMode = false;
                        askAgain = false;
                        messages.Clear();
                        //todo: add progress to drawing
                    }
                    else {
                        askAgain = true;
                        //todo: hacer nueva pregunta
                    }
                    //todo: send score to girl
                    Debug.Log("Calificación obtenida: " + scores.Last());
                }
            }

            print(newMessage.Content);
            messages.Add(newMessage);

            /*

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
            */
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("Tecla T presionada");
                StartRecording();
            }
            else if (Input.GetKeyUp(KeyCode.T))
            {
                Debug.Log("Tecla T no presionada");
                EndRecording();           
            }
        }
    }
}   
