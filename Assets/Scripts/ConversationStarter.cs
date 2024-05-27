using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Samples.Whisper;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] Whisper whisper;
    public ObjectDetection objectDetection;
    private List<string> detectedObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        detectedObjects = objectDetection.GetDetectedObjects();
    }
    
    public IEnumerator StartConversation()
    {

        yield return StartCoroutine(objectDetection.DetectObjects());

        int randomIndex = Random.Range(0, detectedObjects.Count);

        string randomObject = detectedObjects[randomIndex];

        objectDetection.EmptyDetectedObjects();

        var task = whisper.GenerateImaginativeQuestion(randomObject, Whisper.QuestionMode.OBJECT);

        yield return new WaitUntil(() => task.IsCompleted);

    }
}
