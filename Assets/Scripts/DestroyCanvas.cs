using Scripts.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    public Conversation conversation;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!conversation.playing)
        {
            Destroy(this.gameObject);
        }

    }
}