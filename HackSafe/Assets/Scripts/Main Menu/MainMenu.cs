using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using System;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button startGameButton;
    [SerializeField] Button continoueGameButton;
    [SerializeField] Button exitGameButton;
    [SerializeField] Button settingButton;
    
    [SerializeField] OptionsOverlay settingsOverlay;
    [SerializeField] PopupMessageOverlay popupMessageOverla;

    protected static string exitMessageKey = "exitSystemMessage_key";

    // Start is called before the first frame update
    void Start()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener (() => loadSceneForNewGame ());
        }

        if (startGameButton != null)
        {
            continoueGameButton.onClick.AddListener (() => loadSceneForContinoue ());
        }

        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener (() => showPopupMessageOverlay (exitMessageKey, exitGame));
        }

        if (settingButton != null)
        {
            settingButton.onClick.AddListener (() => settingsOverlay.ShowOverlay ());
        }

        continoueGameButton.gameObject.SetActive (checkIfSaveFileExists ());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void loadSceneForContinoue ()
    {
        SceneManager.LoadScene ("Loading");
    }

    protected void loadSceneForNewGame ()
    {
        string fullPath = Path.Combine (FileDataHandler.DataDirPath, FileDataHandler.DataFileName);

        if (File.Exists (fullPath))
        {
            File.Delete (fullPath);
        }

        SceneManager.LoadScene ("Loading");
    }

    protected bool checkIfSaveFileExists ()
    {
        bool result = false;

        string fullPath = Path.Combine (FileDataHandler.DataDirPath, FileDataHandler.DataFileName);

        if (File.Exists (fullPath))
        {
            result = true;
        }

        return result;
    }

    protected void showPopupMessageOverlay (string messageKey, Action aproveAction)
    {
        popupMessageOverla.Inicialize (messageKey, aproveAction);
    }

    protected void exitGame ()
    {
        Application.Quit ();
    }
}
