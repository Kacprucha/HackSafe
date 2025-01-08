using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassiveTerminalElement : MonoBehaviour
{
    [SerializeField] Text textToShow;
    [SerializeField] RectTransform parentRectTransform;

    static int MaxCharactersInResponse = 44;

    protected float basicHight = 0;

    public void UpdateText(string newtext, bool black = false, bool ajustTextSize = true)
    {
        if (black)
        {
            Color color = Color.black;
            textToShow.color = color;
        }
        textToShow.text = newtext;

        if (parentRectTransform != null && ajustTextSize)
            StartCoroutine (ajustSize ());
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

    protected bool isTextOverflowing (int lengthOfText)
    {
        return lengthOfText > MaxCharactersInResponse;
    }

    IEnumerator ajustSize ()
    {
        yield return new WaitForEndOfFrame ();

        int lengthOfResponse = textToShow.text.Length;

        if (basicHight == 0)
            basicHight = parentRectTransform.sizeDelta.y;

        parentRectTransform.sizeDelta = new Vector2 (parentRectTransform.sizeDelta.x, basicHight);

        while (isTextOverflowing (lengthOfResponse))
        {
            parentRectTransform.sizeDelta = new Vector2 (parentRectTransform.sizeDelta.x, parentRectTransform.sizeDelta.y + (basicHight / 2) + 12);

            lengthOfResponse -= MaxCharactersInResponse;
        }
    }
}
