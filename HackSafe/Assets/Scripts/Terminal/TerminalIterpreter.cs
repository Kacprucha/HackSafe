using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    Ssh,
    Update,
    Install
}

public enum TerminalState
{
    Normal,
    WaitingForInput,
    WaitingForSudoPassword,
    WaitingForConfirmation
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
    protected TerminalState terminalState = TerminalState.Normal;
    
    protected List<TypeOfPrpgram> programsToInstall = new List<TypeOfPrpgram> ();

    protected GameState gameState;

    public InputOperator PlayerInputHandler
    {
        get { return playerInputHandler; }
    }

    void Start ()
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

    public void GneratePassiveTermialResponse (string response)
    {
        generateResponseForInput (response);
    }

    public PassiveTerminalElement GneratePassiveTermialResponseWithPossibleUpdate (string response)
    {
        return generateResponseForInputWithPossibleUpdate (response);
    }

    void HandleInput (string inputText)
    {
        if (gameState == null && GameState.instance != null)
            gameState = GameState.instance;

        Debug.Log ("User input: " + inputText);

        GameObject newPlayerCommaned = Instantiate (playerCommandPrefab);
        newPlayerCommaned.GetComponent<PassiveTerminalElement> ().UpdateText (inputText, terminalState == TerminalState.WaitingForSudoPassword);
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

    private PassiveTerminalElement generateResponseForInputWithPossibleUpdate (string response)
    {
        GameObject newTerminalResponse = Instantiate (terminalResponsePrefab);
        newTerminalResponse.GetComponent<PassiveTerminalElement> ().UpdateText (response);
        PassiveTerminalElement passiveTerminalElement = newTerminalResponse.GetComponent<PassiveTerminalElement> ();
        newTerminalResponse.transform.SetParent (this.gameObject.transform);
        newTerminalResponse.transform.localScale = new Vector3 (1, 1, 1);

        queue.Push (newTerminalResponse);

        while (checkIfPopIsNeeded ())
        {
            Destroy (queue.Pop ());
        }

        return passiveTerminalElement;
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
        bool sudoUsed = false;

        if (commend == "sudo")
        {
            commend = input.Split (' ')[1];
            int firstSpaceIndex = input.IndexOf (' ');

            if (firstSpaceIndex != -1)
                arguments = input.Substring (firstSpaceIndex + 1).Split (' ');

            sudoUsed = true;
        }

        int argumentsAmmount = arguments.Length - 1;

        if (checkIfContinuNeededForCommand (currentCommand))
        {
            switch (currentCommand)
            {
                case Commands.Update:
                    commend = "update";
                    break;

                case Commands.Install:
                    if (terminalState == TerminalState.WaitingForSudoPassword)
                        commend = "install";
                    else if (terminalState == TerminalState.WaitingForConfirmation)
                        commend = "install confirm";
                    break;
            }
        }

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

            case "apt":
                if (argumentsAmmount > 0)
                {
                    if (arguments[1] == "install")
                    {
                        if (argumentsAmmount - 1 > 0)
                        {
                            currentCommand = Commands.Install;
                            continoueOnProtectedAction (sudoUsed, "apt install");

                            for (int i = 2; i < arguments.Length; i++)
                            {
                                switch (arguments[i])
                                {
                                    case "bruteForce":
                                        programsToInstall.Add (TypeOfPrpgram.brutForse);

                                        break;

                                    case "rainbowTables":
                                        programsToInstall.Add (TypeOfPrpgram.rainbowTables);

                                        break;

                                    case "dictionaryAttack":
                                        programsToInstall.Add (TypeOfPrpgram.dictionaryAttack);

                                        break;
                                }
                            }
                        }
                        else
                        {
                            generateResponseForInput ("apt install: too few arguments!");
                        }
                    }
                    else if (arguments[1] == "update")
                    {
                        if (argumentsAmmount - 1 == 0)
                        {
                            currentCommand = Commands.Update;
                            continoueOnProtectedAction (sudoUsed, "apt update");
                        }
                        else
                        {
                            generateResponseForInput ("apt update: too many arguments!");
                        }
                    }
                    else
                    {
                        generateResponseForInput ("apt: " + arguments[1] + " not found");
                    }
                }
                else
                {
                    generateResponseForInput ("apt: too few arguments!");
                }

                break;

            case "update":
                if (arguments.Length == 1 && gameState.GetPlayerInfo ().PlayerComputer.CheckIfGivenPasswordIsCorrect (arguments[0]))
                {
                    updateAction ();
                }
                else if (terminalState == TerminalState.WaitingForSudoPassword)
                {
                    generateResponseForInput ("update: permission denied");
                    playerInputHandler.ChangeColourOfText (false);
                    terminalState = TerminalState.Normal;
                    currentCommand = Commands.NotFound;
                }
                else
                {
                    generateResponseForInput ("Command 'update' not found, did you mean command 'apt update'");
                }

                break;

            case "install":
                if (gameState.GetPlayerInfo ().PlayerComputer.CheckIfGivenPasswordIsCorrect (arguments[0]))
                {
                    bool allProgramsAllowed = false;

                    foreach (TypeOfPrpgram program in programsToInstall)
                    {
                        if (TerminalMenager.CheckIfPlayerCanDownloadProgram (program))
                        {
                            allProgramsAllowed = true;
                        }
                        else
                        {
                            generateResponseForInput ("apt install: you don't have access to install " + program + "or this programm does not exist");
                            programsToInstall = new List<TypeOfPrpgram> ();
                            allProgramsAllowed = false;
                            break;
                        }
                    }

                    if (allProgramsAllowed)
                    {
                        List<TypeOfPrpgram> tempPrograms = new List<TypeOfPrpgram> (programsToInstall);
                        foreach (TypeOfPrpgram program in tempPrograms)
                        {
                            if (TerminalMenager.CheckIfPlayerDownloadedProgram (program))
                            {
                                generateResponseForInput ("apt install: " + program + " is already installed");
                                programsToInstall.Remove (program);
                            }
                        }

                        if (programsToInstall.Count > 0)
                        {
                            installAction (programsToInstall, true);
                        }
                        else
                        {
                            PlayerInputHandler.ChangeColourOfText (false);
                            terminalState = TerminalState.Normal;
                            currentCommand = Commands.NotFound;
                        }
                    }
                }
                else
                {
                    generateResponseForInput ("Command 'install' not found, did you mean command 'apt install'");
                }

                break;

            case "install confirm":
                if (arguments.Length == 1 && TerminalMenager.CheckIfResponseIsYes (arguments[0]))
                {
                    installAction (programsToInstall, false);
                    programsToInstall = new List<TypeOfPrpgram> ();
                }
                else
                {
                    generateResponseForInput ("apt install: instalation aborted");
                    terminalState = TerminalState.Normal;
                    currentCommand = Commands.NotFound;
                }

                break;

            default:
                generateResponseForInput ("Command '" + commend + "' not found");
                break;
        }

        sudoUsed = false;
    }

    bool checkIfContinuNeededForCommand (Commands command)
    {
        bool result = false;

        if (terminalState != TerminalState.Normal)
        {
            switch (command)
            {
                case Commands.Update:
                    result = terminalState == TerminalState.WaitingForSudoPassword;
                    break;

                case Commands.Install:
                    result = terminalState == TerminalState.WaitingForConfirmation || terminalState == TerminalState.WaitingForSudoPassword;
                    break;

            }
        }

        return result;
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

        currentCommand = Commands.NotFound;
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

        currentCommand = Commands.NotFound;
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

        currentCommand = Commands.NotFound;
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
                        if (playerFilesystem.FindNode (SystemHelper.GetPathWithoutLastSegment (newFilePath)) != null)
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

        currentCommand = Commands.NotFound;
    }

    void continoueOnProtectedAction (bool ifSudoUsed, string nameOfAction)
    {
        if (ifSudoUsed)
        {
            generateResponseForInput ("[sudo] password for " + gameState.GetPlayerInfo ().PlayerComputer.Username + ":");
            terminalState = TerminalState.WaitingForSudoPassword;

            playerInputHandler.ChangeColourOfText (true);
        }
        else
        {
            generateResponseForInput (nameOfAction + ": permission denied");
            currentCommand = Commands.NotFound;
        }
    }

    private void updateAction ()
    {
        playerInputHandler.ChangeIteractibilityOfInputField (false);
        StartCoroutine (TerminalMenager.GenerateTerminalResponseForUpdate (this));
        terminalState = TerminalState.Normal;
        currentCommand = Commands.NotFound;
    }

    void installAction (List<TypeOfPrpgram> programsToInstall, bool beforeAproveFaze)
    {
        if (beforeAproveFaze)
        {
            playerInputHandler.ChangeIteractibilityOfInputField (false);
            StartCoroutine (TerminalMenager.GenerateTermianlResponseForInstall (this, programsToInstall, beforeAproveFaze));
            terminalState = TerminalState.WaitingForConfirmation;
        }
        else
        {
            playerInputHandler.ChangeIteractibilityOfInputField (false);
            StartCoroutine (TerminalMenager.GenerateTermianlResponseForInstall (this, programsToInstall, beforeAproveFaze));

            foreach (TypeOfPrpgram program in programsToInstall)
            {
                gameState.GetPlayerInfo ().ProgramesDownloaded[program] = true;
            }

            terminalState = TerminalState.Normal;
            currentCommand = Commands.NotFound;
        }
    }
}
