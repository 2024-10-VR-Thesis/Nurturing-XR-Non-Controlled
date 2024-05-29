using Scripts.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Samples.Whisper;
using System.Linq;
using System.Threading.Tasks;
using System;

public class EndGame : MonoBehaviour
{
    public Conversation conversation;
    public Whisper whisper;
    public AudioManager audio;
    public TMP_Text questionTvText;
    public TMP_Text answerTvText;
    public TMP_Text scoreTvText;
    public TMP_Text endgameTvText;
    public int razon;
    private int endPlayed;

    // Start is called before the first frame update
    void Start()
    {
        endgameTvText.text = "";
        razon = 0;
        endPlayed = 0;
    }

    public async void finish()
    {
        while (!conversation.playing && endPlayed == 0)
        {
            endPlayed++;
            audio.endgameVoice(razon);
            double promedio = Math.Round(whisper.scores.Average(), 2);
            await Task.Delay(5000);
            DeleteAllTexts();
            endgameTvText.text = "The End \n  \n Your score average was: " + promedio;
        }
        /*
        if (!conversation.playing && endPlayed == 0)
        {
            endPlayed++;
            audio.endgameVoice(razon);
            double promedio = whisper.scores.Average();
            await Task.Delay(5000);
            DeleteAllTexts();
            endgameTvText.text = "The End \n  \n Your score average was: " + promedio;
        }
        */
    }


    // Update is called once per frame
    void Update()
    {
        finish();
    }

    private void DeleteAllTexts()
    {
        questionTvText.text = "";
        answerTvText.text = "";
        scoreTvText.text = "";
    }
}