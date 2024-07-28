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

    void mailButtonClicked ()
    {
        if (OnMailButtonClicked != null)
        {
            OnMailButtonClicked ();
        }
    }
}
