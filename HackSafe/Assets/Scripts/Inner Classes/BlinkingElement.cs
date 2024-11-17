using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingElement : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float blinkInterval = 0.5f;
    [SerializeField] float minAlpha = 0.2f;
    [SerializeField] float maxAlpha = 1f;        

    private bool isBlinking = false;

    void Start ()
    {
        if (image != null)
        {
            Color initialColor = image.color;
            initialColor.a = maxAlpha; 
            image.color = initialColor;
        }
    }

    public void StartBlinking ()
    {
        if (!isBlinking && image != null)
        {
            StartCoroutine (Blinking ());
        }
    }

    public void StopBlinking ()
    {
        isBlinking = false;
        if (image != null)
        {
            Color resetColor = image.color;
            resetColor.a = maxAlpha; 
            image.color = resetColor;
        }
    }

    private IEnumerator Blinking ()
    {
        isBlinking = true;

        while (isBlinking)
        {
            yield return StartCoroutine (FadeToAlpha (minAlpha));

            yield return StartCoroutine (FadeToAlpha (maxAlpha));
        }
    }

    private IEnumerator FadeToAlpha (float targetAlpha)
    {
        Color color = image.color;
        float startAlpha = color.a;

        float elapsed = 0f;
        while (elapsed < blinkInterval)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp (startAlpha, targetAlpha, elapsed / blinkInterval);
            image.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        image.color = color;
    }
}
