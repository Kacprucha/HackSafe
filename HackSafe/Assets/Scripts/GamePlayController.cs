using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour, IDataPersistance
{
    [SerializeField] TopPanel topPanel;

    [SerializeField] RegisterUserOverlay registerUserOverlay;
    [SerializeField] EmialOverlay emailOverlay;

    [SerializeField] NetworkSymulatorView networkSymulatorView;
    [SerializeField] SystemVariablesView systemVariablesView;

    [SerializeField] TerminalIterpreter terminalIterpreter;

    List<ProgramLogic> programLogicList = new List<ProgramLogic> ();

    protected GameState gameState;

    // Start is called before the first frame update
    void Start ()
    {
        
    }

    void OnEnable ()
    {
        topPanel.OnMailButtonClicked += ChangeVisibilityOfEmailOverlay;

        registerUserOverlay.OnSaveButtonClicked += InicializaPlayer;
        emailOverlay.OnSetEmailViewButtonClicked += actionOnReadingEmail;

        terminalIterpreter.OnInjectPorgramLogic += injectMethodsToProgramLogic;
    }

    void OnDisable ()
    {
        topPanel.OnMailButtonClicked -= ChangeVisibilityOfEmailOverlay;

        registerUserOverlay.OnSaveButtonClicked -= InicializaPlayer;
        emailOverlay.OnSetEmailViewButtonClicked -= actionOnReadingEmail;

        terminalIterpreter.OnInjectPorgramLogic -= injectMethodsToProgramLogic;

        if (programLogicList.Count > 0)
        {
            foreach (ProgramLogic pL in programLogicList)
            {
                pL.OnRunProgram -= addSystemVariables;
                pL.OnStopPorgram -= dealateSystemVariables;
            }
        }
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
            else
            {
                networkSymulatorView.GenerateCommpanyLayOut ();
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

        topPanel.SetMailNotification (true);

        yield return new WaitForEndOfFrame ();
    }

    protected void actionOnReadingEmail (int emailId, bool emailRead)
    {
        EmailMenager.CheckIfEmailNeedAnyAction (emailId, emailRead);

        switch (emailId)
        {
            case 0:
                networkSymulatorView.GenerateCommpanyLayOut ();
                break;
        }
    }

    protected void addSystemVariables (string programName, int cpu, int ram, int storage)
    {
        systemVariablesView.AddProgram (programName, cpu, ram, storage);
    }

    protected void dealateSystemVariables (string programName)
    {
        systemVariablesView.DelateProgram (programName);
    }

    protected void injectMethodsToProgramLogic (ProgramLogic programLogic)
    {
        programLogic.OnRunProgram += addSystemVariables;
        programLogic.OnStopPorgram += dealateSystemVariables;
    }
}
