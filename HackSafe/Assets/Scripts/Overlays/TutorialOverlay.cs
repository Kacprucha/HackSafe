using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOverlay : DraggableOverlay
{
    static public string TutorialKey = "Tutorial";

    public delegate void TutorialFirstClosedHandler ();
    public event TutorialFirstClosedHandler OnTutorialFirstClosed;

    public override void Start ()
    {
        base.Start ();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener (() => closeButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CloseOverlay ()
    {
        base.CloseOverlay ();

        if (PlayerPrefs.HasKey (TutorialKey) && PlayerPrefs.GetInt (TutorialKey) == 0)
        {
            PlayerPrefs.SetInt (TutorialKey, 1);

            if (OnTutorialFirstClosed != null)
            {
                OnTutorialFirstClosed ();
            }
        }
    }

    protected void closeButtonClicked ()
    {
        CloseOverlay ();
    }
}
