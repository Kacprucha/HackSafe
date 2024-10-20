using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour, IDataPersistance
{
    [SerializeField] TopPanel topPanel;

    [SerializeField] RegisterUserOverlay registerUserOverlay;
    [SerializeField] EmialOverlay emailOverlay;

    [SerializeField] TerminalIterpreter terminalIterpreter;

    protected GameState gameState;

    // Start is called before the first frame update
    void Start ()
    {
        
    }

    void OnEnable ()
    {
        topPanel.OnMailButtonClicked += ChangeVisibilityOfEmailOverlay;

        registerUserOverlay.OnSaveButtonClicked += InicializaPlayer;
    }

    void OnDisable ()
    {
        topPanel.OnMailButtonClicked -= ChangeVisibilityOfEmailOverlay;

        registerUserOverlay.OnSaveButtonClicked -= InicializaPlayer;
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    public void LoadData (GameData gameData)
    {
        if (!string.IsNullOrEmpty (gameData.PlayerName) && !string.IsNullOrEmpty (gameData.PlayerPasswored) && !string.IsNullOrEmpty (gameData.PlayerIP))
        {
            gameState = new GameState (gameData);
            terminalIterpreter.UpdatePrefix (gameData.PlayerIP);
            checkIfEmailIsNeededToBeSent ();
        }
        else
        {
            registerUserOverlay.gameObject.SetActive (true);
        }
    }

    public void SaveData (ref GameData gameData)
    {
        if (gameState != null)
        {
            gameState.SaveData (ref gameData);
        }
    }

    void InicializaPlayer (string username, string password)
    {
        gameState = new GameState (username, password);

        terminalIterpreter.UpdatePrefix (gameState.GetPlayerInfo ().PlayerComputer.IP);

        registerUserOverlay.gameObject.SetActive (false);

        checkIfEmailIsNeededToBeSent ();
    }

    void ChangeVisibilityOfEmailOverlay ()
    {
        emailOverlay.gameObject.SetActive (!emailOverlay.gameObject.activeSelf);
        topPanel.SetMailNotification (false);
    }

    protected void checkIfEmailIsNeededToBeSent ()
    {
        if (gameState != null && gameState.GetPlayerInfo () != null)
        {
            PlayerInfo player = gameState.GetPlayerInfo ();

            if (player.RecivedEmails != null && player.RecivedEmails.Count == 0)
            {
                StartCoroutine (sentFirstEmial ());
            }
        }
    }

    protected IEnumerator sentFirstEmial ()
    {
        yield return new WaitForSeconds (5);

        DateTime currentTime = DateTime.Now;

        int hour = currentTime.Hour;
        int minute = currentTime.Minute;
        string formattedDate = currentTime.ToString ("dd.MM.yyyy");

        gameState.GetPlayerInfo ().RecivedEmails.Add (EmailMenager.GetEmailOfId (0, formattedDate, hour.ToString ("00") + ":" + minute.ToString ("00"), false));
        EmailMenager.CheckIfEmailNeedAnyAction (0);

        topPanel.SetMailNotification (true);

        yield return new WaitForEndOfFrame ();
    }
}
