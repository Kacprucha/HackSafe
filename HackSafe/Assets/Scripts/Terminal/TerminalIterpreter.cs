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

        queue.Push (newPlayerCommaned);

        while (checkIfPopIsNeeded ())
        {
            Destroy (queue.Pop ());
        }

        findCommendAndAct (inputText);
    }

    private bool checkIfPopIsNeeded ()
    {
        float sizeOfQueue = 0;
        foreach (GameObject _object in queue.GetArryOfObjectsInQueue ())
        {
            sizeOfQueue += _object.GetComponent<RectTransform> ().sizeDelta.y;
        }

        float siezOfCointainer = this.gameObject.GetComponent<RectTransform> ().rect.height;

        return siezOfCointainer < sizeOfQueue;
    }

    private void generateResponseForInput (string response)
    {
        GameObject newTerminalResponse = Instantiate (terminalResponsePrefab);
        newTerminalResponse.GetComponent<PassiveTerminalElement> ().UpdateText (response);
        newTerminalResponse.transform.SetParent (this.gameObject.transform);
        newTerminalResponse.transform.localScale = new Vector3 (1, 1, 1);

        queue.Push (newTerminalResponse);

        while (checkIfPopIsNeeded ())
        {
            Destroy (queue.Pop ());
        }
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

        string[] arguments = input.Split ();
        int argumentsAmmount = arguments.Length - 1;

        switch (commend)
        {
            case "cd":
                currentCommand = Commands.Cd;
                continouCdAction (arguments, argumentsAmmount);
                break;

            case "ls":
                currentCommand = Commands.Ls;
                continouLsAction (arguments, argumentsAmmount);
                break;

            case "pwd":
                currentCommand = Commands.Pwd;
                generateResponseForInput (SystemHelper.GetCurrentDirectoryOfPlayerFileSystem ());
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
                continouMkdirAction (arguments, argumentsAmmount);
                break;

            case "touch":
                currentCommand = Commands.Touch;
                continouTouchAction (arguments, argumentsAmmount);
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

    void continouCdAction (string[] arguments, int argumentsAmmount)
    {
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

    void continouLsAction (string[] arguments, int argumentsAmmount)
    {
        List<string> childs = new List<string> ();
        FileSystem playerFilesystem = gameState.GetPlayerInfo ().PlayerComputer.FileSystem;

        if (argumentsAmmount < 2)
        {
            if (argumentsAmmount == 0)
                childs = playerFilesystem.ListChildOfCurrentDirectory ();
            else
            {
                if (SystemHelper.CheckIfPathHasCorrectSyntex (arguments[1], !arguments[1].StartsWith ("/"))) 
                {
                    if (arguments[1].StartsWith ("/"))
                    {
                        if (playerFilesystem.FindNode (arguments[1]) != null)
                            childs = playerFilesystem.ListChildOfGivenPath (arguments[1]);
                        else
                            generateResponseForInput ("ls: cannot show elements of ‘" + arguments[1] + "’: No such direcotry");
                    }
                    else
                    {
                        string fullPath = "";
                        if (SystemHelper.GetCurrentDirectoryOfPlayerFileSystem () != "/")
                            fullPath = SystemHelper.GetCurrentDirectoryOfPlayerFileSystem () + "/" + arguments[1];
                        else
                            fullPath = "/" + arguments[1];

                        if (playerFilesystem.FindNode (fullPath) != null)
                            childs = playerFilesystem.ListChildOfGivenPath (fullPath);
                        else
                            generateResponseForInput ("ls: cannot show elements of ‘" + arguments[1] + "’: No such direcotry");
                    }
                }
                else
                {
                    generateResponseForInput ("ls: cannot show elements of ‘" + arguments[1] + "’: Incorrect syntex");
                }
            }

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
        else
        {
            generateResponseForInput ("ls: too many arguments!");
        }
    }

    void continouMkdirAction (string[] arguments, int argumentsAmmount)
    {
        if (argumentsAmmount != 1)
        {
            generateResponseForInput ("mkdir: number of arguments has to be one!");
        }
        else
        {
            string newFilePath = arguments[1];
            FileSystem playerFilesystem = gameState.GetPlayerInfo ().PlayerComputer.FileSystem;

            if (SystemHelper.CheckIfPathHasCorrectSyntex (newFilePath, !newFilePath.StartsWith ("/")))
            {
                if (newFilePath.StartsWith ("/"))
                {
                    if (playerFilesystem.FindNode (newFilePath) == null)
                        playerFilesystem.CreateNode (newFilePath, true);
                    else
                        generateResponseForInput ("mkdir: cannot create directory ‘" + newFilePath + "’: File exists");
                }
                else
                {
                    if (playerFilesystem.FindNode (SystemHelper.GetCurrentDirectoryOfPlayerFileSystem () + "/" + newFilePath) == null)
                        playerFilesystem.CreateNode (newFilePath, true, true);
                    else
                        generateResponseForInput ("mkdir: cannot create directory ‘" + newFilePath + "’: File exists");
                }
            }
            else
            {
                generateResponseForInput ("mkdir: cannot create directory ‘" + newFilePath + "’: Incorrect syntex");
            }
        }
    }

    void continouTouchAction (string[] arguments, int argumentsAmmount)
    {
        if (argumentsAmmount != 1)
        {
            generateResponseForInput ("touch: number of arguments has to be one!");
        }
        else
        {
            string newFilePath = arguments[1];
            FileSystem playerFilesystem = gameState.GetPlayerInfo ().PlayerComputer.FileSystem;

            if (SystemHelper.CheckIfPathHasCorrectSyntex (newFilePath, !newFilePath.StartsWith ("/")))
            {
                if (newFilePath.StartsWith ("/"))
                {
                    if (playerFilesystem.FindNode (newFilePath) == null)
                    {
                        if (playerFilesystem.FindNode(SystemHelper.GetPathWithoutLastSegment(newFilePath)) != null)
                            playerFilesystem.CreateNode (newFilePath, false);
                        else
                            generateResponseForInput ("touch: cannot touch ‘" + newFilePath + "’: No such directory");
                    }
                    else
                        generateResponseForInput ("touch: cannot touch ‘" + newFilePath + "’: File exists");
                }
                else
                {
                    string fullPath = "";
                    if (SystemHelper.GetCurrentDirectoryOfPlayerFileSystem () != "/")
                        fullPath = SystemHelper.GetCurrentDirectoryOfPlayerFileSystem () + "/" + newFilePath;
                    else
                        fullPath = "/" + newFilePath;

                    if (playerFilesystem.FindNode (fullPath) == null)
                    {
                        if (playerFilesystem.FindNode (SystemHelper.GetPathWithoutLastSegment (fullPath)) != null)
                            playerFilesystem.CreateNode (newFilePath, false, true);
                        else
                            generateResponseForInput ("touch: cannot touch ‘" + newFilePath + "’: No such directory");
                    }
                    else
                        generateResponseForInput ("touch: cannot touch ‘" + newFilePath + "’: File exists");
                }
            }
            else
            {
                generateResponseForInput ("touch: cannot touch ‘" + newFilePath + "’: Incorrect syntex");
            }
        }
    }
}
