using Scripts.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPosition : MonoBehaviour
{
    public Conversation conversation;
    GameObject gameObject;

    public void transformPosition()
    {
        gameObject.transform.position = new Vector3((float)0.974, (float)0.736, (float)-8.03);
        gameObject.transform.Rotate(new Vector3(0, 90, 0));
    }

    void Update()
    {
        if (conversation.playing) { transformPosition(); }
    }
}