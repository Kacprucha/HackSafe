using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    [SerializeField] Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener (() => CloseButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseButtonClicked ()
    {
        this.gameObject.SetActive (false);
    }
}
