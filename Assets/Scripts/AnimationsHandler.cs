using UnityEngine;
using Scripts.Conversation;
using Samples.Whisper;

public class AnimationsHandler : MonoBehaviour
{
    Animator anim;
    Conversation conversation;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        conversation = GetComponent<Conversation>();

    }

    public void setRating(int rating)
    {
        if (rating > 7)
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
        }
        else if (rating < 4)
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