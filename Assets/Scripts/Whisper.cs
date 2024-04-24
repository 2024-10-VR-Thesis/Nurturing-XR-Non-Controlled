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
using Scripts.DrawingProgress;
using Amazon;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using Unity.VisualScripting;
using System.Security.Cryptography;
using Scripts.Conversation;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Text message;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private OpenAIApi openai = new OpenAIApi();
        private OpenAIApi openAI = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as 5 y/o child. Dont break character. Dont ever mention you are an AI model. Create an imaginative question based on an object or a previous answer, just the question. ";
        private string scorePrompt = "Score the answer given to a question from 1-10, dont write words just the score based on a 5 y/o child pov and if the answer and question are in the same context. ";
        public List<int> scores = new List<int>();
        public TextToSpeech tts;
        private CreateChatCompletionRequest requestAI;
        public AnimationsHandler animationsHandler;
        public DrawingProgress drawingProgress;
        private string question;
        public Conversation conversation;


        private void Start()
        {
            //drawingProgress = GetComponent<DrawingProgress>();
            //GenerateImaginativeQuestion("Pillow", QuestionMode.OBJECT);
            //Debug.Log("Inicio");
        }

        public void StartRecording()
        {
            conversation.listening = true;
#if !UNITY_WEBGL
            clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
#endif
        }

        public async void EndRecording()
        {

#if !UNITY_WEBGL
            Microphone.End(null);
#endif

            byte[] data = SaveWav.Save(fileName, clip);

            // Obtener la transcripción del audio
            string transcribedText = await GetAudioTranscription(data);

            await scoreAnswer(transcribedText);

            // Enviar la transcripción a ChatGPT para obtener la pregunta imaginativa
            if (scores.Count > 0 && scores.Last() < 7)
            {
                await GenerateImaginativeQuestion(transcribedText, QuestionMode.ASK_AGAIN);
                conversation.listening = false;
            }
            else
            {
                messages.Clear();
                conversation.listening = false;
            }

        }

        public async Task<string> GetAudioTranscription(byte[] audioData)
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
                return -1; //Si no se encuentra un número en el comentario se devuelve -1 por lo que no se imprime en consola
            }
        }

        public async Task GenerateImaginativeQuestion(string transcribedText, QuestionMode mode) //no es necesariamente transcripcion, tambien es objeto
        {
            Debug.Log("--------------------LLEGO------------------------");

            ChatMessage newMessage = new ChatMessage();
            //newMessage.Content = transcribedText;
            newMessage.Role = "user";
            var questionPrompt = prompt;
            if (mode == QuestionMode.ASK_AGAIN)
            {
                var previousAnswer = transcribedText;
                questionPrompt += "Previous answer: " + previousAnswer;
            }
            else
            {
                var objeto = transcribedText;
                questionPrompt += "Object: " + objeto;
            }

            newMessage.Content = questionPrompt;
            messages.Add(newMessage);

            requestAI = new CreateChatCompletionRequest();
            requestAI.Messages = messages;
            requestAI.Model = "gpt-3.5-turbo";

            var aiResponse = await openAI.CreateChatCompletion(requestAI);

            if (aiResponse.Choices != null && aiResponse.Choices.Count > 0)
            {
                var chatResponse = aiResponse.Choices[0].Message;
                messages.Add(chatResponse);
                string text = chatResponse.Content;
                question = text;
                tts.texttospeech(text);
            }

            
        }

        void Update()
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

        private async Task scoreAnswer(string transcribedAnswer)
        {
            ChatMessage newMessage = new ChatMessage();
            var fullScorePrompt = scorePrompt;
            var answer = transcribedAnswer;
            fullScorePrompt += "Question: " + question + ". Answer: " + answer;
            newMessage.Content = fullScorePrompt;
            newMessage.Role = "user";
            messages.Add(newMessage);

            requestAI = new CreateChatCompletionRequest();
            requestAI.Messages = messages;
            requestAI.Model = "gpt-3.5-turbo";

            var aiResponse = await openAI.CreateChatCompletion(requestAI);

            if (aiResponse.Choices != null && aiResponse.Choices.Count > 0)
            {
                var chatResponse = aiResponse.Choices[0].Message;
                string text = chatResponse.Content;
                int rating = await ExtractRatingFromResponse(text);

                if (rating == -1)
                {
                    if (scores.Count >= 1)
                    {
                        List<double> provisional = scores.Select(x => (double)x).ToList();
                        rating = (int)Math.Floor(provisional.Average());
                    }
                    else
                    {
                        rating = 1;
                    }
                }
                scores.Add(rating);
                animationsHandler.setRating(rating);

                Debug.Log("Calificación obtenida: " + scores.Last());

                if (scores.Last() >= 7) //todo: ver si ajustamos rangos
                {
                    drawingProgress.increaseIndex();
                }
                //messages.Clear()
            }
        }

        public enum QuestionMode
        {
            ASK_AGAIN,
            OBJECT
        }
    }
}
