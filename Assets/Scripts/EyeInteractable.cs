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

    // Add this method to initialize the conversation object
    void Start()
    {
        conversation = GetComponent<Conversation>();
        if (conversation == null)
        {
            Debug.LogError("Conversation component not found!");
        }
    }

    void Update()
    {
        if (IsHovered)
        {
            if (conversation != null && !(conversation.talking || conversation.listening))
            {
                OnObjectHover?.Invoke(gameObject);
                name = gameObject.name;
                whisper.GenerateImaginativeQuestion(name, Whisper.QuestionMode.OBJECT);
                Debug.Log(name);
            }
        }
    }
}
