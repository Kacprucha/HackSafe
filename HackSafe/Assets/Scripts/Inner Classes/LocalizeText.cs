using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeText : MonoBehaviour
{
    [SerializeField] string Key;

    // Start is called before the first frame update
    void Start()
    {
        Text text = GetComponent<Text> ();
        if (text != null)
        {
            text.text = LocalizationManager.Instance.GetLocalizedValue (Key);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
