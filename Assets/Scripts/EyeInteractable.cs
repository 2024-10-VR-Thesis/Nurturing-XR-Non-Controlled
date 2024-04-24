using UnityEngine;
using UnityEngine.Events;
using Samples.Whisper;
using Scripts.Conversation;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    public UnityEvent<GameObject> OnObjectHover;

    [SerializeField]
    Whisper whisper;

    [SerializeField]
    Conversation conversation;

    void Start()
    {
        conversation = GetComponent<Conversation>();
        if (conversation == null)
        {
            Debug.LogError("Conversation component not found!");
        }
    }

    async void Update()
    {
        if (IsHovered)
        {
           Debug.Log(gameObject.name);
            if (conversation != null && !(conversation.talking || conversation.listening))
            {
                await whisper.GenerateImaginativeQuestion(name, Whisper.QuestionMode.OBJECT);
                OnObjectHover?.Invoke(gameObject);
                name = gameObject.name;
            }
        }
    }
}
