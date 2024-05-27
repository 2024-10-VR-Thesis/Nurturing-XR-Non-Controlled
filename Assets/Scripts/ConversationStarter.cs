using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Samples.Whisper;
using TMPro;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] Whisper whisper;
    public ObjectDetection objectDetection;
    private List<string> detectedObjects;
    public QuestionCountdown questionCountdown;
    public TMP_Text questionTvText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public IEnumerator StartConversation()
    {
        questionTvText.text = "Question: (Please wait until objects are done being detected)";
        
        yield return StartCoroutine(objectDetection.DetectObjects());
        detectedObjects = objectDetection.GetDetectedObjects();

        int randomIndex = Random.Range(0, detectedObjects.Count-1);

        string randomObject = detectedObjects[randomIndex];

        objectDetection.EmptyDetectedObjects();

        StartCoroutine(questionCountdown.UpdateTime());
        
        yield return new WaitForSeconds(15);

        var task = whisper.GenerateImaginativeQuestion(randomObject, Whisper.QuestionMode.OBJECT);

        yield return new WaitUntil(() => task.IsCompleted);

    }
}
