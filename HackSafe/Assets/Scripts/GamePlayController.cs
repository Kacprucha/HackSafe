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
    }

    void ChangeVisibilityOfEmailOverlay ()
    {
        emailOverlay.gameObject.SetActive (!emailOverlay.gameObject.activeSelf);
    }
}
