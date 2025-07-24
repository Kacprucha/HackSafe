using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsOverlay : DraggableOverlay
{
    [SerializeField] Button nextLeftButton;
    [SerializeField] Button nextRightButton;
    [SerializeField] Text languageLabel;

    [SerializeField] Button saveChanges;

    [SerializeField] Button backToMainMenu;

    string[] languages;
    int indexOfLanguage = -1;

    public override void Start ()
    {
        base.Start ();

        inicialize ();

        if (nextLeftButton != null)
        {
            nextLeftButton.onClick.AddListener (() => nextLeftButtonClicked ());
        }

        if (nextRightButton != null)
        {
            nextRightButton.onClick.AddListener (() => nextRightButtonClicked ());
        }

        if (saveChanges != null)
        {
            saveChanges.onClick.AddListener (() => saveLanguageChanges ());
        }

        if (backToMainMenu != null)
        {
            backToMainMenu.onClick.AddListener (() => goBackToMaineMenu ());
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void OnEnable ()
    {
        inicialize ();
    }

    protected void inicialize ()
    {
        languages = LocalizationManager.Languages;

        for (int i = 0; i < languages.Length; i++)
        {
            if (languages[i] == LocalizationManager.Instance.CurrentLanguage)
            {
                indexOfLanguage = i;
                break;
            }
        }

        languageLabel.text = LocalizationManager.Instance.CurrentLanguage;
    }

    protected void nextLeftButtonClicked ()
    {
        indexOfLanguage--;

        if (indexOfLanguage < 0)
            indexOfLanguage = languages.Length - 1;

        languageLabel.text = languages[indexOfLanguage];
    }

    protected void nextRightButtonClicked ()
    {
        indexOfLanguage++;

        if (indexOfLanguage >= languages.Length)
            indexOfLanguage = 0;

        languageLabel.text = languages[indexOfLanguage];
    }

    protected void saveLanguageChanges ()
    {
        LocalizationManager.Instance.SetLanguage (languages[indexOfLanguage]);
        PlayerPrefs.SetString (LocalizationManager.LanguageKey, languages[indexOfLanguage]);

        if (SceneManager.GetActiveScene ().name == "Gameplay") 
        {
            SceneManager.LoadScene ("Loading");
        }
        else
        {
            SceneManager.LoadScene ("MainMenu");
        }
    }

    protected void goBackToMaineMenu ()
    {
        DataPersistanceMenager.Instance.SaveGame ();
        SceneManager.LoadScene ("MainMenu");
    }
}
