using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanel : MonoBehaviour
{
    [SerializeField] Button systemButton;
    [SerializeField] Button notesButton;
    [SerializeField] Button glosaryButton;

    [SerializeField] GameObject systemButtonBackground;
    [SerializeField] GameObject notesButtonBackground;

    [SerializeField] GameObject systemPanel;
    [SerializeField] GameObject notesPanel;
    [SerializeField] GameObject glosary;

    public delegate void GlosaryButtonHandler (bool visible);
    public event GlosaryButtonHandler OnGlosaryButtonClicked;

    protected bool glosaryVisible = false;

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

        if (glosaryButton != null)
        {
            glosaryButton.onClick.AddListener (() => openGlosary ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        glosaryVisible = glosary.activeSelf;   
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

    protected void openGlosary ()
    {
        if (OnGlosaryButtonClicked != null)
        {
            OnGlosaryButtonClicked (glosaryVisible);
        }
    }
}
