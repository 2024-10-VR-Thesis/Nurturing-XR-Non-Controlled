using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Conversation;
using UnityEngine.SocialPlatforms.Impl;


public class AnimationsHandler : MonoBehaviour
{
    Animator anim;
    Conversation conversation;
    
    bool isRecording;
    bool isTalking;
    int rating;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        rating = GetComponent<int>();
        conversation = GetComponent<Conversation>();
    }

    private void setRating()
    {
        if(rating > 7) {
            if (conversation.soBad_v > 0)
            {
                conversation.soBad_v--;
            } else if (conversation.bad_v > 0)
            {
                conversation.bad_v--;
            }
        } else if (rating < 4) {
            conversation.soBad_v++;
        } else
        {
            conversation.bad_v++;
        }

    }

    void Update()
    {
        setRating();
    }
}
