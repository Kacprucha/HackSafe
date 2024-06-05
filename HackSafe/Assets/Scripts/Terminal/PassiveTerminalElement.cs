using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveTerminalElement : MonoBehaviour
{
    [SerializeField] Text textToShow;

    public void UpdateText(string newtext)
    {
        textToShow.text = newtext;
    }

    public void SetTextToBestFit ()
    {
        textToShow.resizeTextForBestFit = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
