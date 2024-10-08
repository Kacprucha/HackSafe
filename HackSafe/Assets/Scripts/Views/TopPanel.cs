using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    [SerializeField] Button homeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button mailButton;
    [SerializeField] Button exitButton;

    [SerializeField] GameObject mailNotification;

    public delegate void MailButtonTopPanelOverlayHandler ();
    public event MailButtonTopPanelOverlayHandler OnMailButtonClicked;

    // Start is called before the first frame update
    void Start()
    {
        if (mailButton != null)
        {
            mailButton.onClick.AddListener (() => mailButtonClicked ());
        }
    }

    public void SetMailNotification (bool state)
    {
        if (mailNotification != null)
        {
            mailNotification.SetActive (state);
        }
    }

    void mailButtonClicked ()
    {
        if (OnMailButtonClicked != null)
        {
            OnMailButtonClicked ();
        }
    }
}
