using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Conversation;
using UnityEngine.SocialPlatforms.Impl;
using Samples.Whisper;
using System.Linq;



public class AnimationsHandler : MonoBehaviour
{
    Animator anim;
    Whisper Whisper;
    Conversation conversation;

    bool isRecording;
    bool isTalking;
    int rating;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        conversation = GetComponent<Conversation>();

    }

    public void setRating(int rating)
    {
        if(rating > 7)
        {
            if (conversation.soBad_v > 0)
            {
                conversation.soBad_v--;
                anim.SetInteger("SoBad_v", anim.GetInteger("SoBad_v") - 1);
            }
            else if (conversation.bad_v > 0)
            {
                conversation.bad_v--;
                anim.SetInteger("Bad_v", anim.GetInteger("Bad_v") - 1);
            }
        }else if (rating < 4)
        {
            conversation.soBad_v++;
            anim.SetInteger("SoBad_v", anim.GetInteger("SoBad_v") + 1);
        }
        else
        {
            conversation.bad_v++;
            anim.SetInteger("Bad_v", anim.GetInteger("Bad_v") + 1);
        }
    }

    public void setBooleans()
    {
        anim.SetBool("Talking", conversation.talking);
        anim.SetBool("Listening", conversation.listening);
    }

    void Update()
    {
        setBooleans();
    }
}
