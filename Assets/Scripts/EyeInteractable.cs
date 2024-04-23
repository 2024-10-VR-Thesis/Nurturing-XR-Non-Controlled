using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Samples.Whisper;
using Scripts.Conversation;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    [SerializeField]
    private UnityEvent<GameObject> OnObjectHover;
    
    Whisper whisper;
    Conversation conversation;
    



    void Start() { }


    void Update()
    {
        if(IsHovered)
        {
            if( !(conversation.talking || conversation.listening) ){
                OnObjectHover?.Invoke(gameObject);
                name = gameObject.name;
                whisper.GenerateImaginativeQuestion(name, Whisper.QuestionMode.OBJECT);
            }
            
        }
    }
}
