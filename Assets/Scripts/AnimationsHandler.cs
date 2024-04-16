using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Samples.Whisper;
using Scripts.TexToSpeech;
using UnityEngine.SocialPlatforms.Impl;


public class AnimationsHandler : MonoBehaviour
{
    Animator anim;
    
    bool isRecording;
    bool isTalking;
    string rating;

    Whisper whisper;
    TextToSpeech speech;   

    void Start()
    {
        anim = GetComponent<Animator>();
        rating = GetComponent<string>();
        //isRecording = whisper.getRecordingState();
        //isTalking = speech.talking;
    }

    private void setRating()
    {
        if(isRecording) { rating = "Listening"; }
        if(isTalking) { rating = "Talking"; }
        if( int.Parse(rating) < 4 ) { rating = "So Bad Answer"; }
        if( int.Parse(rating) < 7 ) { rating = "Bad Answer"; }
        if( int.Parse(rating) >= 7 ) { rating = "Good Answer"; }
    }
        
    void Update()
    {
        setRating();
        if(rating == "Good Answer")
        {
            anim.SetBool("Talking",         false);
            anim.SetBool("Listening",       false);
            anim.SetBool("Good Answer",     true);
            anim.SetBool("Bad Answer",      false);
            anim.SetBool("So Bad Answer",   false);
        }
        if (rating == "Bad Answer")
        {
            anim.SetBool("Talking",         false);
            anim.SetBool("Listening",       false);
            anim.SetBool("Good Answer",     false);
            anim.SetBool("Bad Answer",      true);
            anim.SetBool("So Bad Answer",   false);
        }
        if (rating == "So Bad Answer")
        {
            anim.SetBool("Talking",         false);
            anim.SetBool("Listening",       false);
            anim.SetBool("Good Answer",     false);
            anim.SetBool("Bad Answer",      false);
            anim.SetBool("So Bad Answer",   true);
        }
        if (rating == "Talking")
        {
            anim.SetBool("Talking",         true);
            anim.SetBool("Listening",       false);
            anim.SetBool("Good Answer",     false);
            anim.SetBool("Bad Answer",      false);
            anim.SetBool("So Bad Answer",   false);
        }
        if (rating == "Listening")
        {
            anim.SetBool("Talking",         false);
            anim.SetBool("Listening",       true);
            anim.SetBool("Good Answer",     false);
            anim.SetBool("Bad Answer",      false);
            anim.SetBool("So Bad Answer",   false);
        }
    }
}
