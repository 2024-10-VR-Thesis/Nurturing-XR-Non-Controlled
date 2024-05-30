using System.Collections;
using UnityEngine;
using Scripts.Conversation;
using TMPro;
using Scripts.DrawingProgress;

public class ClockHandler : MonoBehaviour
{
    public Conversation conversation;

    private bool start = false;

    public TMP_Text questionTvText;
    public TMP_Text timeText;

    public ConversationStarter conversationStarter;
    public DrawingProgress drawingProgress;
    public EndGame endGame;

    private int minutes = 5;
    private int seconds = 0;

    private void Start()
    {
       
    }

    private void Update()
    {
        if (start && (minutes == 0 && seconds == 0))
        {  
            if (drawingProgress.GetDrawnObjects() < 4)
            {
                endGame.razon = 1;
            }
            conversation.playing = false;
        }
    }

    public IEnumerator StartTime()
    {
        start = true;

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