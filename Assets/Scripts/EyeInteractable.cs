using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using Whisper.whisper;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    [SerializeField]
    private UnityEvent<GameObject> OnObjectHover;

    //Whisper anya = new Whisper();

    void Start() { }


    void Update()
    {
        if(IsHovered)
        {
            //if( !(anya.isTalking || anya.isListening) ){
            // OnObjectHover?.Invoke(gameObject)
            // name = gameObject.name;
            // anya.addObject(name);
            //}

            name = gameObject.name;
            
        }
        else
        {
            
        }
        
    }
}
