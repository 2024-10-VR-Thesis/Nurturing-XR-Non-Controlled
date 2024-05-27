using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Scripts.Conversation;
using TMPro;

public class ClockHandler : MonoBehaviour
{
    Conversation conversation;

    private bool start = false;

    public TMP_Text questionTvText;
    public TMP_Text timeText;

    //public ConversationStarter conversationStarter;

    private int minutes = 1;
    private int seconds = 0;

    private void Start()
    {
        StartCoroutine(StartTime());
    }

    private void Update()
    {
        if (start && (minutes == 0 && seconds == 0))
        {
            conversation.playing = false;

        }
    }

    public IEnumerator StartTime()
    {
        start = true;
        questionTvText.text = "Question: (Please wait)";
        //conversationStarter.StartConversation();
        while (minutes != 0 || seconds != 0)
        {  
            seconds -= 1;

            if (seconds == -1)
            {
                seconds = 59;
                minutes -= 1;
            }

            timeText.text = "0" + minutes + ":" + (seconds <= 9 ? "0" : "") + seconds;

            yield return new WaitForSeconds(1);
        }
    }

    public bool GetStart()
    {
        return start;
    }
}