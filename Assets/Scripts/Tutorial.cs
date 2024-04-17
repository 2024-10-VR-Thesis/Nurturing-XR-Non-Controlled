using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public Behaviour tutorialCanvas;
    void Start()
    {
       // tutorialCanvas.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialCanvas.enabled)
        {
            Destroy(this.gameObject, 10f);
        }
    }
}
