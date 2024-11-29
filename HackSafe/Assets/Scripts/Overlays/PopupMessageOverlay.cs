using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessageOverlay : DraggableOverlay
{
    [SerializeField] Text message;

    [SerializeField] Button approveButton;
    [SerializeField] Button disaproveBurron;

    // Start is called before the first frame update
    public override void Start ()
    {
        base.Start ();

        if (disaproveBurron != null)
        {
            disaproveBurron.onClick.AddListener (() => onDisapproveButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicialize (string messageKey, Action onApprove)
    {
        message.text = LocalizationManager.Instance.GetLocalizedValue (messageKey);

        approveButton.onClick.AddListener (() => onApprove ());

        ShowOverlay ();
    }

    protected void onDisapproveButtonClicked ()
    {
        CloseOverlay ();
    }
}
