using Scripts.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Samples.Whisper;
using System.Linq;
using System.Threading.Tasks;

public class EndGame : MonoBehaviour
{
    public Conversation conversation;
    public Whisper whisper;
    public AudioManager audio;
    public TMP_Text questionTvText;
    public TMP_Text answerTvText;
    public TMP_Text scoreTvText;
    public TMP_Text endgameTvText;
    public int razon;
    private int contador;

    // Start is called before the first frame update
    void Start()
    {
        endgameTvText.text = "";
        contador = 0;
        razon = 0;
    }

    // Update is called once per frame
    async void Update()
    {
        if (!conversation.playing && contador == 0)
        {
            audio.endgameVoice(razon);
            double promedio = whisper.scores.Average();
            await Task.Delay(5000);
            DeleteAllTexts();
            endgameTvText.text = "The End \n  \n Your score average was: " + promedio;
            contador++;
        }
    }

    private void DeleteAllTexts()
    {
        questionTvText.text = "";
        answerTvText.text = "";
        scoreTvText.text = "";
    }
}