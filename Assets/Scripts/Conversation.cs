using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Conversation
{
    public sealed class Conversation : MonoBehaviour
    {
        public static Conversation instance;
        public bool talking { get; set; }
        public bool listening { get; set; }
        public bool drawing { get; set; }
        public bool playing { get; set; }

        public int bad_v { get; set; }
        public int soBad_v { get; set; }

        public List<string> askedObjects = new List<string>();
        public EndGame endGame;

        private Conversation()
        {
        }

        public static Conversation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Conversation();
                }
                return instance;
            }
        }


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
            talking = true; playing = true; bad_v = 0; soBad_v = 0;
            //User regulates interaction. Set First prompt after finishing intro
        }

        void EndGame()
        {

        }

    }
}
