using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Scripts.Conversation;

public class MinuteHandMovement : MonoBehaviour
{
    Conversation conversation;
    public float totalRotationAngle = 360f;
    public float durationMinutes = 0.5f;

    private float rotationSpeed;
    private float durationSeconds;
    private float elapsedTime = 0f;

    public Transform pivotPoint;

    private void Start()
    {
        durationSeconds = durationMinutes * 60;
        rotationSpeed = totalRotationAngle / durationSeconds;
    }

    private void Update()
    {

        if (elapsedTime < durationSeconds)
        {
            transform.RotateAround(pivotPoint.position, Vector3.left, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
        }
        /*
        else
        {
            conversation.playing = false;
        }
        */
    }


}