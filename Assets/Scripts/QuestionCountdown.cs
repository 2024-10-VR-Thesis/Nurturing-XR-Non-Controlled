using System.Collections;
using UnityEngine;
using TMPro;

public class QuestionCountdown : MonoBehaviour
{
    public TMP_Text countdownTvText;
    private int countdownTime = 20;
    private bool questionReady { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        questionReady = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator UpdateTime()
    {
        while (countdownTime > 0 || questionReady)
        {
            yield return new WaitForSeconds(1);
            countdownTime -= 1;

            countdownTvText.text = $"Question: (In about {countdownTime} sec" + (countdownTime == 1 ? "" : "s") + ")";
        }

        countdownTime = 20;
    }

}