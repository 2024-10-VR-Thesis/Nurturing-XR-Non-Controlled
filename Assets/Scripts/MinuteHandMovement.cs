using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Scripts.Conversation;
using TMPro;

public class MinuteHandMovement : MonoBehaviour
{
    Conversation conversation;
    public float durationMinutes;

    private float rotationSpeed;
    private float durationSeconds;
    private float elapsedTime = 0f;

    public Transform pivotPoint;

    private bool start = false;

    public TMP_Text questionTvText;

    public EndGame endGame;

    private void Start()
    {
        durationSeconds = durationMinutes * 60;
        rotationSpeed = 360f / durationSeconds;
    }

    private void Update()
    {
        if (start && elapsedTime < durationSeconds)
        {
            transform.RotateAround(pivotPoint.position, Vector3.left, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
        }
        if (elapsedTime >= durationSeconds)
        {
            endGame.razon = 1;
            conversation.playing = false;
        }
    }

    public void StartTime()
    {
        start = true;
        questionTvText.text = "Question: (Please look around to find an object)";

    }

    public bool GetStart()
    {
        return start;
    }
}