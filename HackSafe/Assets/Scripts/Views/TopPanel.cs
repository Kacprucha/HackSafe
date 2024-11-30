using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    [SerializeField] Button homeButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button saveButton;
    [SerializeField] Button mailButton;
    [SerializeField] Button exitButton;

    [SerializeField] GameObject mailNotification;

    public delegate void MailButtonHandler ();
    public event MailButtonHandler OnMailButtonClicked;

    public delegate void SettingsButtonHandler ();
    public event SettingsButtonHandler OnSettingsButtonClicked;

    public delegate void HomeButtonHandler ();
    public event HomeButtonHandler OnHomeButtonClicked;

    public delegate void ExitButtonHandler (string messageKey, Action aproveAction);
    public event ExitButtonHandler OnExitButtonClicked;

    protected static string exitMessageKey = "exitSystemMEssage_key";
    protected Color savingColor = new Color (0.2313726f, 0.9254902f, 0.2705882f, 1);

    protected bool mailButtonAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        if (mailButton != null)
        {
            mailButton.onClick.AddListener (() => mailButtonClicked ());
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener (() => settingsButtonClicked ());
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener (() => exitButtonClicked ());
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener (() => saveButtonClicked ());
        }

        if (homeButton != null)
        {
            homeButton.onClick.AddListener (() => homeButtonClicked ());
        }
    }

    private void Update ()
    {
        GameState gameState = GameState.instance;

        if (gameState != null && gameState.GetPlayerInfo () != null)
        {
            foreach (Email email in gameState.GetPlayerInfo ().RecivedEmails)
            {
                if (!email.EmailRead && !mailButtonAnimating)
                {
                    mailButtonAnimating = true;
                    StartCoroutine (smoothColorTransition (mailButton.gameObject.GetComponent<Image> (), Color.red, Color.black, 1f, true));
                    return;
                }
            }
        }
    }

    void mailButtonClicked ()
    {
        if (OnMailButtonClicked != null)
        {
            OnMailButtonClicked ();
        }
    }

    void settingsButtonClicked ()
    {
        if (OnSettingsButtonClicked != null) 
        {
            OnSettingsButtonClicked ();
        }
    }

    void exitButtonClicked ()
    {
        if (OnExitButtonClicked != null)
        {
            OnExitButtonClicked (exitMessageKey, onExitButtonActionProceed);
        }
    }

    void saveButtonClicked ()
    {
        DataPersistanceMenager.Instance.SaveGame ();
        saveButton.gameObject.GetComponent<Image> ().color = savingColor;

        StartCoroutine (smoothColorTransition (saveButton.gameObject.GetComponent<Image> (), savingColor, Color.black, 0.8f));
    }

    void homeButtonClicked ()
    {
        if (OnHomeButtonClicked != null)
        {
            OnHomeButtonClicked ();
        }
    }

    protected void onExitButtonActionProceed ()
    {
        DataPersistanceMenager.Instance.SaveGame ();
        SceneManager.LoadScene ("MainMenu");
    }

    protected IEnumerator smoothColorTransition (Image targetImage, Color fromColor, Color toColor, float transitionTime, bool isItMailButton = false)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            targetImage.color = Color.Lerp (fromColor, toColor, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame (); 
        }

        targetImage.color = toColor;

        if (isItMailButton)
            mailButtonAnimating = false;
    }
}
