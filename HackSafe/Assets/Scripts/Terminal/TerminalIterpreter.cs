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
    Ssh,
    Update,
    Install,
    BruteForce,
    Clear,
    DictionaryAttack,
    Exit,
    ManInTheMiddle,
    RainbowAttack
}

public enum TerminalState
{
    Normal,
    WaitingForInput,
    WaitingForSudoPassword,
    WaitingForConfirmation,
    WaitingForPassword
}

public class TerminalIterpreter : MonoBehaviour
{
    [SerializeField] InputOperator playerInputHandler;
    [SerializeField] GameObject playerCommandPrefab;
    [SerializeField] GameObject terminalResponsePrefab;

    [SerializeField] Text prefix;

    public InputOperator PlayerInputHandler { get { return playerInputHandler; } }

    public string TerminalIP { get { return ip; } }

    public FileSystem TermianlFileSystem { get { return terminalFileSystem; } }

    public TerminalState TerminalState
    {
        get { return terminalState; }
        set { terminalState = value; }
    }

    public Commands CurrentCommand
    {
        get { return currentCommand; }
        set { currentCommand = value; }
    }

    public bool ProgramIsRunning
    {
        get { return programIsRunning; }
        set { programIsRunning = value; }
    }

    static int MaxCapasityOfTermianl = 19;

    protected GameState gameState;
    protected FileSystem terminalFileSystem;
    protected string ip;

    protected FIFOQueue<GameObject> queue = new FIFOQueue<GameObject> ();
    protected Commands currentCommand = Commands.NotFound;
    protected TerminalState terminalState = TerminalState.Normal;
    protected bool programIsRunning = false;

    protected AptLogic aptLogic;
    protected BruteForceLogic bruteForceLogic;
    protected DicionaryAttackLogic dicionaryAttackLogic;
    protected SshLogic sshLogic;
    protected ScpLogic scpLogic;
    protected ManInTheMiddleLogic manInTheMiddleLogic;
    protected FakeSignatureLogic fakeSignatureLogic;
    protected RainbowAttakcLogic rainbowAttakcLogic;

    public delegate void InjectProgramLogicHandler (ProgramLogic programLogic);
    public event InjectProgramLogicHandler OnInjectPorgramLogic;

    public delegate void InjectSshLogicHandler (SshLogic sshLogic);
    public event InjectSshLogicHandler OnInjectSshLogic;

    public delegate void InjectManInTheMiddleHandler (ManInTheMiddleLogic manInTheMiddleLogic);
    public event InjectManInTheMiddleHandler OnInjectManInTheMiddleLogic;

    public delegate void InjectionFakeSignatureHandler (FakeSignatureLogic fakeSignatureLogic);
    public event InjectionFakeSignatureHandler OnInjectFakeSignatureLogic;

    void Start ()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnInputEntered += HandleInput;
        }
    }

    void Update ()
    {
        if (programIsRunning)
        {
            if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKeyDown (KeyCode.C))
            {
                programIsRunning = false;
            }
        }
        else
        {
            if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKeyDown (KeyCode.L))
            {
                CleanTerminal ();
            }
            else if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKeyDown (KeyCode.D)) 
            {
                closeSshConection ();
            }
        }
    }

    public void UpdateFileSystem (string ip, FileSystem fileSystem = null)
    {
        if (ip != null)
        {
            prefix.text = ">" + ip + ":~$";

            this.ip = ip;

            if (fileSystem != null)
            {
                if (gameState.GetPlayerInfo ().PlayerComputer.IP == ip)
                {
                    terminalFileSystem = gameState.GetPlayerInfo ().PlayerComputer.FileSystem;
                }
                else
                {
                    terminalFileSystem = gameState.FindComputerOfIP (ip).FileSystem;
                }
            }
            else
            {
                terminalFileSystem = fileSystem;
            }
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
        inicialize ();

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

    public IEnumerator RefreshLayout ()
    {
        yield return new WaitForEndOfFrame ();
        LayoutRebuilder.ForceRebuildLayoutImmediate (this.gameObject.GetComponent<RectTransform> ());

        while (checkIfPopIsNeeded ())
        {
            Destroy (queue.Pop ());
        }
    }

    public void CleanTerminal ()
    {
        int ammountOfLinesInTermianl = queue.Count;

        for (int i = 0; i < ammountOfLinesInTermianl; i++)
        {
            Destroy (queue.Pop ());
        }
    }

    public void StopAptProgram ()
    {
        if (aptLogic != null)
            aptLogic.StopProgram ();
    }

    protected void inicialize ()
    {
        if (gameState == null && GameState.instance != null)
        {
            gameState = GameState.instance;

            if (bruteForceLogic == null)
            {
                GameObject gameObject = new GameObject ("BruteForceElement");
                gameObject.AddComponent<BruteForceLogic> ();
                gameObject.GetComponent<BruteForceLogic> ().Inicialize (this, playerInputHandler);
                bruteForceLogic = gameObject.GetComponent<BruteForceLogic> ();
                OnInjectPorgramLogic (bruteForceLogic);
            }

            if (aptLogic == null)
            {
                GameObject gameObject = new GameObject ("AptElement");
                gameObject.AddComponent<AptLogic> ();
                gameObject.GetComponent<AptLogic> ().Inicialize (this, playerInputHandler);
                aptLogic = gameObject.GetComponent<AptLogic> ();
                OnInjectPorgramLogic (aptLogic);
            }

            if (dicionaryAttackLogic == null)
            {
                GameObject gameObject = new GameObject ("DicionaryAttackElement");
                gameObject.AddComponent<DicionaryAttackLogic> ();
                gameObject.GetComponent<DicionaryAttackLogic> ().Inicialize (this, playerInputHandler);
                dicionaryAttackLogic = gameObject.GetComponent<DicionaryAttackLogic> ();
                OnInjectPorgramLogic (dicionaryAttackLogic);
            }

            if (sshLogic == null)
            {
                GameObject gameObject = new GameObject ("SshElement");
                gameObject.AddComponent<SshLogic> ();
                gameObject.GetComponent<SshLogic> ().Inicialize (this, playerInputHandler);
                sshLogic = gameObject.GetComponent<SshLogic> ();
                OnInjectSshLogic (sshLogic);
            }

            if (scpLogic == null)
            {
                GameObject gameObject = new GameObject ("ScpElement");
                gameObject.AddComponent<ScpLogic> ();
                gameObject.GetComponent<ScpLogic> ().Inicialize (this, playerInputHandler);
                scpLogic = gameObject.GetComponent<ScpLogic> ();
            }

            if (manInTheMiddleLogic == null)
            {
                GameObject gameObject = new GameObject ("ManInTheMiddleElement");
                gameObject.AddComponent<ManInTheMiddleLogic> ();
                gameObject.GetComponent<ManInTheMiddleLogic> ().Inicialize (this, playerInputHandler);
                manInTheMiddleLogic = gameObject.GetComponent<ManInTheMiddleLogic> ();
                OnInjectManInTheMiddleLogic (manInTheMiddleLogic);
                OnInjectPorgramLogic (manInTheMiddleLogic);
            }

            if (fakeSignatureLogic == null)
            {
                GameObject gameObject = new GameObject ("FakeSignatureElement");
                gameObject.AddComponent<FakeSignatureLogic> ();
                gameObject.GetComponent<FakeSignatureLogic> ().Inicialize (this, playerInputHandler);
                fakeSignatureLogic = gameObject.GetComponent<FakeSignatureLogic> ();
                OnInjectFakeSignatureLogic (fakeSignatureLogic);
                OnInjectPorgramLogic (fakeSignatureLogic);
            }

            if (rainbowAttakcLogic == null)
            {
                GameObject gameObject = new GameObject ("RainbowAttackElement");
                gameObject.AddComponent<RainbowAttakcLogic> ();
                gameObject.GetComponent<RainbowAttakcLogic> ().Inicialize (this, playerInputHandler);
                rainbowAttakcLogic = gameObject.GetComponent<RainbowAttakcLogic> ();
                OnInjectPorgramLogic (rainbowAttakcLogic);
            }
        }

        if (terminalFileSystem == null)
        {
            terminalFileSystem = gameState.GetPlayerInfo ().PlayerComputer.FileSystem;
        }
    }

    private bool checkIfPopIsNeeded ()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate (this.gameObject.GetComponent<RectTransform> ());

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

                case Commands.Ssh:
                    if (terminalState == TerminalState.WaitingForPassword)
                        commend = "ssh password";
                    else if (terminalState == TerminalState.WaitingForConfirmation)
                        commend = "ssh confirm";

                    break;

                case Commands.Scp:
                    if (terminalState == TerminalState.WaitingForPassword)
                        commend = "scp password";
                    else if (terminalState == TerminalState.WaitingForConfirmation)
                        commend = "scp confirm";

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
                continouCatAction (arguments, argumentsAmmount);
                break;

            case "scp":
                currentCommand = Commands.Scp;
                scpLogic.ContinouOnScpAction (arguments);
                break;

            case "scp confirm":
                scpLogic.ScpAction (arguments, SshConectionStage.SshKeyGeneration);

                break;

            case "scp password":
                scpLogic.ScpAction (arguments, SshConectionStage.SshPassword);

                break;

            case "ssh":
                sshLogic.ContinouSSHAction (arguments);
                break;

            case "ssh confirm":
                sshLogic.SshAction (arguments, SshConectionStage.SshKeyGeneration);

                break;

            case "ssh password":
                sshLogic.SshAction (arguments, SshConectionStage.SshPassword);

                break;

            case "clear":
                currentCommand = Commands.Clear;
                CleanTerminal ();

                break;

            case "apt":
                aptLogic.ContinoueOnAptAction (sudoUsed, arguments);

                break;

            case "update":
                aptLogic.ContinoueOnUpdateAction (arguments);

                break;

            case "install":
                aptLogic.ContinoueOnInstallAction (arguments);

                break;

            case "install confirm":
                aptLogic.ContinoueOnInstallConfirmAction (arguments);

                break;

            case "bruteForce":
                bruteForceLogic.ContinoueOnBruteForceAction (arguments);

                break;

            case "dictionaryAttack":
                dicionaryAttackLogic.ContinoueOnDictionaryAction (arguments);

                break;

            case "exit":
                currentCommand = Commands.Exit;
                closeSshConection ();

                break;

            case "manInTheMiddle":
                manInTheMiddleLogic.ContinoueOnManInTheMiddleAction (arguments);

                break;

            case "fakeSignature":
                fakeSignatureLogic.ContinoueOnFakeSignatureAction (arguments);
                
                break;

            case "rainbowAttack":
                rainbowAttakcLogic.ContinoueOnRainbowTableAction (arguments);
                break;

            default:
                generateResponseForInput ("Command '" + commend + "' not found");
                break;
        }

        sudoUsed = false;
    }

    protected bool checkIfContinuNeededForCommand (Commands command)
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

                case Commands.Ssh:
                case Commands.Scp:
                    result = terminalState == TerminalState.WaitingForConfirmation || terminalState == TerminalState.WaitingForPassword;
                    break;
            }
        }

        return result;
    }

    protected void continouCdAction (string[] arguments, int argumentsAmmount)
    {
        if (argumentsAmmount > 1)
        {
            generateResponseForInput ("cd: too many arguments!");
        }
        else
        {
            if (argumentsAmmount == 0)
            {
                terminalFileSystem.ChangeDirectory ("/");
            }
            else
            {
                if (!terminalFileSystem.ChangeDirectory (arguments[1]))
                {
                    generateResponseForInput ("cd: " + arguments[1] + " No such file or directory");
                }
            }
        }

        currentCommand = Commands.NotFound;
    }

    protected void continouLsAction (string[] arguments, int argumentsAmmount)
    {
        List<string> childs = new List<string> ();

        if (argumentsAmmount < 2)
        {
            if (argumentsAmmount == 0)
                childs = terminalFileSystem.ListChildOfCurrentDirectory ();
            else
            {
                if (SystemHelper.CheckIfPathHasCorrectSyntex (arguments[1], !arguments[1].StartsWith ("/")))
                {
                    if (arguments[1].StartsWith ("/"))
                    {
                        if (terminalFileSystem.FindNode (arguments[1]) != null)
                            childs = terminalFileSystem.ListChildOfGivenPath (arguments[1]);
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

                        if (terminalFileSystem.FindNode (fullPath) != null)
                            childs = terminalFileSystem.ListChildOfGivenPath (fullPath);
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

    protected void continouMkdirAction (string[] arguments, int argumentsAmmount)
    {
        if (argumentsAmmount != 1)
        {
            generateResponseForInput ("mkdir: number of arguments has to be one!");
        }
        else
        {
            string newFilePath = arguments[1];

            if (SystemHelper.CheckIfPathHasCorrectSyntex (newFilePath, !newFilePath.StartsWith ("/")))
            {
                if (newFilePath.StartsWith ("/"))
                {
                    if (terminalFileSystem.FindNode (newFilePath) == null)
                        terminalFileSystem.CreateNode (newFilePath, true);
                    else
                        generateResponseForInput ("mkdir: cannot create directory ‘" + newFilePath + "’: File exists");
                }
                else
                {
                    if (terminalFileSystem.FindNode (SystemHelper.GetCurrentDirectoryOfPlayerFileSystem () + "/" + newFilePath) == null)
                        terminalFileSystem.CreateNode (newFilePath, true, true);
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

    protected void continouTouchAction (string[] arguments, int argumentsAmmount)
    {
        if (argumentsAmmount != 1)
        {
            generateResponseForInput ("touch: number of arguments has to be one!");
        }
        else
        {
            string newFilePath = arguments[1];

            if (SystemHelper.CheckIfPathHasCorrectSyntex (newFilePath, !newFilePath.StartsWith ("/")))
            {
                if (newFilePath.StartsWith ("/"))
                {
                    if (terminalFileSystem.FindNode (newFilePath) == null)
                    {
                        if (terminalFileSystem.FindNode (SystemHelper.GetPathWithoutLastSegment (newFilePath)) != null)
                            terminalFileSystem.CreateNode (newFilePath, false);
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

                    if (terminalFileSystem.FindNode (fullPath) == null)
                    {
                        if (terminalFileSystem.FindNode (SystemHelper.GetPathWithoutLastSegment (fullPath)) != null)
                            terminalFileSystem.CreateNode (newFilePath, false, true);
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

    protected void continouCatAction (string[] arguments, int argumentsAmmount)
    {
        if (argumentsAmmount != 1)
        {
            generateResponseForInput ("cat: number of arguments has to be one!");
        }
        else
        {
            TreeNode file;

            if (SystemHelper.CheckIfPathHasCorrectSyntex (arguments[1], !arguments[1].StartsWith ("/")))
            {
                if (arguments[1].StartsWith ("/"))
                {
                    file = terminalFileSystem.FindNode (arguments[1]);

                    if (file != null && !file.IsDirectory)
                    {
                        generateResponseForInput (file.Content);
                        StartCoroutine (RefreshLayout ());
                    }
                    else
                        generateResponseForInput ("cat: " + arguments[1] + " no such file!");
                }
                else
                {
                    file = terminalFileSystem.FindNode (terminalFileSystem.GetPathOfCurrentDirectory () + "/" + arguments[1]);

                    if (file != null && !file.IsDirectory)
                    {
                        generateResponseForInput (file.Content);
                        StartCoroutine (RefreshLayout ());
                    }
                    else
                        generateResponseForInput ("cat: " + arguments[1] + " no such file!");
                }
            }
            else
            {
                generateResponseForInput ("cat: ‘" + arguments[1] + "’: Incorrect syntex");
            }
        }

        currentCommand = Commands.NotFound;
    }

    protected void closeSshConection ()
    {
        if (TerminalIP != gameState.GetPlayerInfo ().PlayerComputer.IP)
        {
            if (gameState.FindComputerOfIP (TerminalIP).IsDataBased)
                sshLogic.DisConnectFromdataBase ();

            generateResponseForInput ("logout");
            generateResponseForInput ($"Connection to {TerminalIP} closed.");

            UpdateFileSystem (gameState.GetPlayerInfo ().PlayerComputer.IP);
        }
        else
        {
            CleanTerminal ();
        }
    }
}
