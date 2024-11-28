using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour, IDataPersistance
{
    [SerializeField] TopPanel topPanel;
    [SerializeField] BottomPanel bottomPanel;

    [SerializeField] RegisterUserOverlay registerUserOverlay;
    [SerializeField] EmailOverlay emailOverlay;
    [SerializeField] ManInTheMiddleOverlay manInTheMiddleOverlay;
    [SerializeField] TimerOverlay timerOverlay;
    [SerializeField] FakeSignatureOverlay fakeSignatureOverlay;
    [SerializeField] GlosaryOverlay glosaryOverlay;
    [SerializeField] OptionsOverlay optionsOverlay;
    [SerializeField] PopupMessageOverlay popupMessageOverlay;

    [SerializeField] NetworkSymulatorView networkSymulatorView;
    [SerializeField] DataBaseView dataBaseView;
    [SerializeField] SystemVariablesView systemVariablesView;
    [SerializeField] TasksListView tasksListView;

    [SerializeField] TerminalIterpreter terminalIterpreter;

    protected List<ProgramLogic> programLogicList = new List<ProgramLogic> ();

    protected GameState gameState;

    protected DraggableOverlay overlayUsingTimer;

    // Start is called before the first frame update
    void Start ()
    {

    }

    void OnEnable ()
    {
        topPanel.OnMailButtonClicked += ChangeVisibilityOfEmailOverlay;
        topPanel.OnSettingsButtonClicked += ChangeVisibilityOfOptionsOverlay;
        topPanel.OnExitButtonClicked += showPopupMessageOverlay;
        bottomPanel.OnGlosaryButtonClicked += changeStateOfGlosaryOverlay;

        registerUserOverlay.OnSaveButtonClicked += InicializaPlayer;
        emailOverlay.OnSetEmailViewButtonClicked += actionOnReadingEmail;
        manInTheMiddleOverlay.OnStartTimer += setUpTimer;
        manInTheMiddleOverlay.OnFinishConectingToStream += onManInTheMiddleAttackSuccess;
        timerOverlay.OnFinishCounting += onTimerFinish;

        tasksListView.OnQuestDone += onQuestTasksDone;

        terminalIterpreter.OnInjectPorgramLogic += injectMethodsToProgramLogic;
        terminalIterpreter.OnInjectSshLogic += injectMethodsToSshLogic;
        terminalIterpreter.OnInjectManInTheMiddleLogic += injectMethodsToManInTheMiddleLogic;
        terminalIterpreter.OnInjectFakeSignatureLogic += injectMethodsToFakeSignatureLogic;
    }

    void OnDisable ()
    {
        topPanel.OnMailButtonClicked -= ChangeVisibilityOfEmailOverlay;
        topPanel.OnSettingsButtonClicked -= ChangeVisibilityOfOptionsOverlay;
        bottomPanel.OnGlosaryButtonClicked -= changeStateOfGlosaryOverlay;

        registerUserOverlay.OnSaveButtonClicked -= InicializaPlayer;
        emailOverlay.OnSetEmailViewButtonClicked -= actionOnReadingEmail;
        manInTheMiddleOverlay.OnStartTimer -= setUpTimer;
        manInTheMiddleOverlay.OnFinishConectingToStream -= onManInTheMiddleAttackSuccess;
        timerOverlay.OnFinishCounting -= onTimerFinish;

        tasksListView.OnQuestDone -= onQuestTasksDone;

        terminalIterpreter.OnInjectPorgramLogic -= injectMethodsToProgramLogic;
        terminalIterpreter.OnInjectSshLogic -= injectMethodsToSshLogic;
        terminalIterpreter.OnInjectManInTheMiddleLogic -= injectMethodsToManInTheMiddleLogic;
        terminalIterpreter.OnInjectFakeSignatureLogic -= injectMethodsToFakeSignatureLogic;

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
        if (emailOverlay.gameObject.activeSelf)
        {
            emailOverlay.CloseOverlay ();
        }
        else
        {
            emailOverlay.ShowOverlay ();
            topPanel.SetMailNotification (false);
        }
    }

    void ChangeVisibilityOfOptionsOverlay ()
    {
        if (optionsOverlay.gameObject.activeSelf)
        {
            optionsOverlay.CloseOverlay ();
        }
        else
        {
            optionsOverlay.ShowOverlay ();
        }
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

    protected void injectMethodsToManInTheMiddleLogic (ManInTheMiddleLogic manInTheMiddleLogic)
    {
        manInTheMiddleLogic.OnShowOverlay += manInTheMiddleOverlay.ShowOverlay;
    }

    protected void injectMethodsToFakeSignatureLogic (FakeSignatureLogic fakeSignatureLogic)
    {
        fakeSignatureLogic.OnShowOverlay += fakeSignatureOverlay.ShowOverlay;
    }

    protected void setUpTimer (DraggableOverlay overlay, float time)
    {
        timerOverlay.ShowOverlay ();
        timerOverlay.SetTimer (time);

        overlayUsingTimer = overlay;
    }

    protected void onTimerFinish ()
    {
        if (overlayUsingTimer != null)
        {
            overlayUsingTimer.CloseOverlay ();
            timerOverlay.CloseOverlay ();

            overlayUsingTimer = null;
            terminalIterpreter.GneratePassiveTermialResponse ("Your actions were noticed! The program was stopped!");
        }
    }

    protected void onManInTheMiddleAttackSuccess ()
    {
        timerOverlay.CloseOverlay ();
    }

    protected void changeStateOfGlosaryOverlay (bool visible)
    {
        if (!visible)
        {
            glosaryOverlay.ShowOverlay ();
        }
        else
        {
            glosaryOverlay.CloseOverlay ();
        }
    }

    protected void showPopupMessageOverlay (string messageKey, Action aproveAction)
    {
        popupMessageOverlay.Inicialize (messageKey, aproveAction);
    }
}
