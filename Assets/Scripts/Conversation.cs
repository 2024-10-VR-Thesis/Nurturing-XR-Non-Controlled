using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Conversation
{
    public class Conversation : MonoBehaviour
{
        private bool talking { get; set; }
        private bool listening { get; set; }
        private bool drawing {  get; set; } 
        private bool playing {  get; set; }
        private int drawingPhase { get; set; }

        void Start()
        {
                StartGame();
        }
        void Update()
        {
            if (!playing)
                {
                    EndGame();
                }
        }

        void StartGame()
        {
            talking = true;
            //TODO: Play the Intro, set Polly message (delay 3.000), Show the instructions
            //User regulates interaction. Set First prompt after finishing intro
        }

        void EndGame()
        {
            //TODO: Stop the game, close the AI interaction, display the results of the session
        }

    }
}
