using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanel : MonoBehaviour
{
    [SerializeField] Button systemButton;
    [SerializeField] Button notesButton;

    [SerializeField] GameObject systemButtonBackground;
    [SerializeField] GameObject notesButtonBackground;

    [SerializeField] GameObject systemPanel;
    [SerializeField] GameObject notesPanel;

    // Start is called before the first frame update
    void Start()
    {
        if (systemButton != null)
        {
            systemButton.onClick.AddListener (() => systemButtonClicked ());
        }

        if (notesButton != null)
        {
            notesButton.onClick.AddListener (() => notesButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void systemButtonClicked ()
    {
        systemButtonBackground.SetActive (true);
        notesButtonBackground.SetActive (false);

        systemPanel.SetActive (true);
        notesPanel.SetActive (false);
    }

    protected void notesButtonClicked ()
    {
        systemButtonBackground.SetActive (false);
        notesButtonBackground.SetActive (true);

        systemPanel.SetActive (false);
        notesPanel.SetActive (true);
    }
}
