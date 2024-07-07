using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Commands
{
    NotFound,
    Cd,
    Ls,
    Pwd,
    Cp,
    Mv,
    Rm,
    Mkdir,
    Touch,
    Cat,
    Scp,
    Ssh
}

public class TerminalIterpreter : MonoBehaviour
{
    [SerializeField] InputOperator playerInputHandler;
    [SerializeField] GameObject playerCommandPrefab;
    [SerializeField] GameObject terminalResponsePrefab;

    [SerializeField] Text prefix;

    static int MaxCapasityOfTermianl = 19;

    public Computer PlayerComputer;

    protected FIFOQueue<GameObject> queue = new FIFOQueue<GameObject> ();
    protected Commands currentCommand = Commands.NotFound;

    protected GameState gameState;

    void Start()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnInputEntered += HandleInput;
        }
    }

    void Update ()
    {

    }

    public void UpdatePrefix (string ip)
    {
        if (ip != null)
        {
            prefix.text = ">" + ip + ":~$";
        }
    }

    void HandleInput (string inputText)
    {
        if (gameState == null && GameState.instance != null)
            gameState = GameState.instance;

        Debug.Log ("User input: " + inputText);

        GameObject newPlayerCommaned = Instantiate (playerCommandPrefab);
        newPlayerCommaned.GetComponent<PassiveTerminalElement> ().UpdateText (inputText);
        newPlayerCommaned.transform.SetParent (this.gameObject.transform);
        newPlayerCommaned.transform.localScale = new Vector3 (1, 1, 1);

        if (queue.Count == MaxCapasityOfTermianl)
        {
            Destroy (queue.Pop ());
        }

        queue.Push (newPlayerCommaned);

        findCommendAndAct (inputText);
    }

    private void generateResponseForInput (string response)
    {
        GameObject newTerminalResponse = Instantiate (terminalResponsePrefab);
        newTerminalResponse.GetComponent<PassiveTerminalElement> ().UpdateText (response);
        newTerminalResponse.transform.SetParent (this.gameObject.transform);
        newTerminalResponse.transform.localScale = new Vector3 (1, 1, 1);

        if (queue.Count == MaxCapasityOfTermianl)
        {
            Destroy (queue.Pop ());
        }

        queue.Push (newTerminalResponse);
    }

    private void findCommend (string input)
    {
        string commend = input.Split (' ')[0];

        switch (commend)
        {
            case "cd":
                currentCommand = Commands.Cd;
                break;

            case "ls":
                currentCommand = Commands.Ls;
                break;

            case "pwd":
                currentCommand = Commands.Pwd;
                break;

            case "cp":
                currentCommand = Commands.Cp;
                break;

            case "mv":
                currentCommand = Commands.Mv;
                break;

            case "rm":
                currentCommand = Commands.Rm;
                break;

            case "mkdir":
                currentCommand = Commands.Mkdir;
                break;

            case "touch":
                currentCommand = Commands.Touch;
                break;

            case "cat":
                currentCommand = Commands.Cat;
                break;

            case "spc":
                currentCommand = Commands.Scp;
                break;

            case "ssh":
                currentCommand = Commands.Ssh;
                break;
        }
    }

    private void findCommendAndAct (string input)
    {
        string commend = input.Split (' ')[0];

        switch (commend)
        {
            case "cd":
                currentCommand = Commands.Cd;
                continouCdAction (input);
                break;

            case "ls":
                currentCommand = Commands.Ls;
                continouLsAction ();
                break;

            case "pwd":
                currentCommand = Commands.Pwd;
                generateResponseForInput (gameState.GetPlayerInfo ().PlayerComputer.FileSystem.GetPathOfCurrentDirectory ());
                break;

            case "cp":
                currentCommand = Commands.Cp;
                break;

            case "mv":
                currentCommand = Commands.Mv;
                break;

            case "rm":
                currentCommand = Commands.Rm;
                break;

            case "mkdir":
                currentCommand = Commands.Mkdir;
                break;

            case "touch":
                currentCommand = Commands.Touch;
                break;

            case "cat":
                currentCommand = Commands.Cat;
                break;

            case "spc":
                currentCommand = Commands.Scp;
                break;

            case "ssh":
                currentCommand = Commands.Ssh;
                break;
        }
    }

    void continouCdAction (string input)
    {
        string[] arguments = input.Split ();
        int argumentsAmmount = arguments.Length - 1;

        if (argumentsAmmount > 1)
        {
            generateResponseForInput ("cd: too many arguments!");
        }
        else
        {
            if (argumentsAmmount == 0)
            {
                gameState.GetPlayerInfo ().PlayerComputer.FileSystem.ChangeDirectory ("/");
            }
            else
            {
                if (!gameState.GetPlayerInfo ().PlayerComputer.FileSystem.ChangeDirectory (arguments[1]))
                {
                    generateResponseForInput ("cd: " + arguments[1] + " No such file or directory");
                }
            }
        }
    }

    void continouLsAction ()
    {
        List<string> childs = gameState.GetPlayerInfo ().PlayerComputer.FileSystem.ListChildOfCurrentDirectory ();

        if (childs.Count > 0)
        {

            string response = "";

            foreach (string child in childs)
            {
                response += child + " ";
            }

            generateResponseForInput (response);
        }
    }
}
