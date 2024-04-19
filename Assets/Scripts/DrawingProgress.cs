using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.DrawingProgress
{
    public class DrawingProgress : MonoBehaviour
    {

        public GameObject[] drawings;
        public float spawnInterval = 5f;
        private float timer = 0f;
        public int index;
        private bool create = false;

        void Start()
        {
            DisableObjects();
            index = -1;
        }

        void Update()
        {
            /* timer += Time.deltaTime;
            print(timer);

            if (timer >= spawnInterval)
            {
                // TODO: set create according to acceptance of replies
                create = true;
                timer = 0f; // Reset the timer
            }

            if (create)
            {
                create = false;
                if (index < drawings.Length)
                {
                    drawings[index].SetActive(true);
                    index += 1;
                }
            } */
            if (index != -1 && index < drawings.Length)
            {
                drawings[index].SetActive(true);
            }
        }

        void DisableObjects()
        {
            foreach (GameObject obj in drawings)
            {
                obj.SetActive(false);
            }
        }

        public void increaseIndex()
        {
            print("increase");
            index += 1;
        }
    }
}
