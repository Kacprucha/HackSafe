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
    [SerializeField] DataBaseView dataBaseView;
    [SerializeField] SystemVariablesView systemVariablesView;
    [SerializeField] TasksListView tasksListView;

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

        tasksListView.OnQuestDone += onQuestTasksDone;

        terminalIterpreter.OnInjectPorgramLogic += injectMethodsToProgramLogic;
        terminalIterpreter.OnInjectSshLogic += injectMethodsToSshLogic;
    }

    void OnDisable ()
    {
        topPanel.OnMailButtonClicked -= ChangeVisibilityOfEmailOverlay;

        registerUserOverlay.OnSaveButtonClicked -= InicializaPlayer;
        emailOverlay.OnSetEmailViewButtonClicked -= actionOnReadingEmail;

        tasksListView.OnQuestDone -= onQuestTasksDone;

        terminalIterpreter.OnInjectPorgramLogic -= injectMethodsToProgramLogic;
        terminalIterpreter.OnInjectSshLogic -= injectMethodsToSshLogic;

        if (programLogicList.Count > 0)
        {
            foreach (ProgramLogic pL in programLogicList)
            {
                pL.OnRunProgram -= addSystemVariables;
                pL.OnStopPorgram -= dealateSystemVariables;
                pL.OnUpdateProgram -= updateSystemVariables;
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

            terminalIterpreter.UpdateFileSystem (gameData.PlayerIP);

            checkIfEmailIsNeededToBeSent ();

            if (gameState.ActiveQuest != null)
                tasksListView.CrateTaskElements (gameState.ActiveQuest.Tasks);
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

        terminalIterpreter.UpdateFileSystem (gameState.GetPlayerInfo ().PlayerComputer.IP);

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
                StartCoroutine (sentEmialAction (0));
            }
            else
            {
                networkSymulatorView.GenerateCommpanyLayOut ();
            }
        }
    }

    protected IEnumerator sentEmialAction (int emailID)
    {
        yield return new WaitForSeconds (5);

        DateTime currentTime = DateTime.Now;

        int hour = currentTime.Hour;
        int minute = currentTime.Minute;
        string formattedDate = currentTime.ToString ("dd.MM.yyyy");

        Quest firstQuest = gameState.GetQuestOfId (emailID);
        EmailData emailData = firstQuest.EmailData;

        emailData.Day = formattedDate;
        emailData.Time = hour.ToString ("00") + ":" + minute.ToString ("00");

        gameState.GetPlayerInfo ().RecivedEmails.Add (EmailMenager.GetEmailFromeData (emailData));

        topPanel.SetMailNotification (true);

        yield return new WaitForEndOfFrame ();
    }

    protected void actionOnReadingEmail (int emailId, bool emailRead)
    {
        if (!emailRead)
        {
            Quest quest = gameState.GetQuestOfId (emailId);
            quest.MakeActionAfterRecivingEmail ();

            foreach (ActionsAfterRecivingEmail action in quest.AfterEmailActions)
            {
                switch (action)
                {
                    case ActionsAfterRecivingEmail.LoadNetworkView:
                        networkSymulatorView.GenerateCommpanyLayOut ();
                        break;
                }
            }

            tasksListView.CrateTaskElements (quest.Tasks);

            gameState.SetActiveQuest (quest);
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

    protected void updateSystemVariables (string programName, int cpu, int ram, int storage)
    {
        systemVariablesView.UpdateProgramInpact (programName, cpu, ram, storage);
    }

    protected void injectMethodsToProgramLogic (ProgramLogic programLogic)
    {
        programLogic.OnRunProgram += addSystemVariables;
        programLogic.OnStopPorgram += dealateSystemVariables;
        programLogic.OnUpdateProgram += updateSystemVariables;

        programLogicList.Add (programLogic);
    }

    protected void injectMethodsToSshLogic (SshLogic sshLogic)
    {
        sshLogic.OnConnectWithdataBase += dataBaseView.Show;
        sshLogic.OnDisconnectWithDataBase += dataBaseView.Hide;
    }

    protected void onQuestTasksDone (int questId)
    {
        if (gameState.GetQuestOfId (questId + 1) != null)
            StartCoroutine (sentEmialAction (questId + 1));
    }
}
