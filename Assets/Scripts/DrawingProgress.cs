using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingProgress: MonoBehaviour
{

    public GameObject[] drawings;
    public float spawnInterval = 5f;
    private float timer = 0f;
    private int index = 0;
    private bool create = false;

    void Start()
    {
        DisableObjects();
    }

    void Update()
    {
        timer += Time.deltaTime;
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
        }
    }

    void DisableObjects()
    {
        foreach (GameObject obj in drawings)
        {
            obj.SetActive(false);
        }
    }
}