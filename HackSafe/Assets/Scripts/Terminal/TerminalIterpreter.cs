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
    Install,
    BruteForce,
    Clear
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
    protected bool programIsRunning = false;

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
        if (programIsRunning) 
        {
            if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKeyDown (KeyCode.C))
            {
                programIsRunning = false;
            }
        }

        if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) && Input.GetKeyDown (KeyCode.L))
        {
            int ammountOfLinesInTermianl = queue.Count;

            for (int i = 0; i < ammountOfLinesInTermianl; i++)
            {
                Destroy (queue.Pop ());
            }
        }
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

    IEnumerator refreshLayout ()
    {
        yield return new WaitForEndOfFrame ();
        LayoutRebuilder.ForceRebuildLayoutImmediate (this.gameObject.GetComponent<RectTransform> ());

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

            case "clear":
                currentCommand = Commands.Clear;

                int ammountOfLinesInTermianl = queue.Count;

                for (int i = 0; i < ammountOfLinesInTermianl; i++)
                {
                    Destroy (queue.Pop ());
                }

                break;

            case "apt":
                continoueOnAptAction (sudoUsed, arguments);

                break;

            case "update":
                continoueOnUpdateAction (arguments);

                break;

            case "install":
                continoueOnInstallAction (arguments);

                break;

            case "install confirm":
                continoueOnInstallConfirmAction (arguments);

                break;

            case "bruteForce":
                continoueOnBruteForceAction (arguments);

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

    protected void continouLsAction (string[] arguments, int argumentsAmmount)
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

    protected void continouMkdirAction (string[] arguments, int argumentsAmmount)
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

    protected void continouTouchAction (string[] arguments, int argumentsAmmount)
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

    protected void continoueOnAptAction (bool sudoUsed, string[] arguments)
    {
        int argumentsAmmount = arguments.Length -1;

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
    }

    protected void continoueOnProtectedAction (bool ifSudoUsed, string nameOfAction)
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

    protected void continoueOnUpdateAction (string[] arguments)
    {
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
    }

    protected void updateAction ()
    {
        playerInputHandler.ChangeIteractibilityOfInputField (false);
        StartCoroutine (TerminalMenager.GenerateTerminalResponseForUpdate (this));
        terminalState = TerminalState.Normal;
        currentCommand = Commands.NotFound;
    }

    protected void continoueOnInstallAction (string[] arguments)
    {
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
    }

    protected void continoueOnInstallConfirmAction (string[] arguments)
    {
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
    }

    protected void installAction (List<TypeOfPrpgram> programsToInstall, bool beforeAproveFaze)
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

    protected void continoueOnBruteForceAction (string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded[TypeOfPrpgram.brutForse])
        {
            if (argumentsAmmount == 0)
            {
                generateResponseForInput ("bruteForce: too few arguments!");
            }
            else if (arguments[1] == "-h" || arguments[1] == "--help")
            {
                generateResponseForInput ("Usage: bruteforce [OPTIONS] <TARGET_IP>");
                generateResponseForInput ("Brute force attack tool for penetration testing. Use this tool responsibly and only on systems you have permission to test.");
                generateResponseForInput ("Options:");
                generateResponseForInput ("-h, --help            Show this help message and exit");
                generateResponseForInput ("-m, --multiplier      Set the multiplier for the attack");
                generateResponseForInput ("Example:");
                generateResponseForInput ("bruteforce 192.168.1.5");
                generateResponseForInput ("bruteforce 192.168.1.100 -m 5");
                generateResponseForInput (" Notes:");
                generateResponseForInput ("  - Ensure the wordlist file exists and contains potential passwords.");
                generateResponseForInput ("  - Brute force attacks can take significant time, especially with complex passwords.");
                generateResponseForInput ("  - The multiplier option is used to increase the number of attempts per second. The default is 1. It can't be more then 300");
                generateResponseForInput ("  - Use at your own risk. Unauthorized use is illegal.");

                StartCoroutine (refreshLayout ());
            }
            else
            {
                bool allGood = true;
                float multiplayer = 1;

                if (argumentsAmmount >= 2)
                {
                    if (arguments[2] == "-m" || arguments[2] == "--multiplier")
                    {
                        if (argumentsAmmount >= 3)
                        {
                            multiplayer = float.Parse (arguments[3]);
                            if (multiplayer > 300)
                            {
                                generateResponseForInput ("bruteForce: multiplier is to big!");
                                allGood = false;
                            }
                            else if (multiplayer < 1)
                            {
                                generateResponseForInput ("bruteForce: multiplier has to be at least 1!");
                                allGood = false;
                            }
                        }
                        else
                        {
                            generateResponseForInput ("bruteForce: too few arguments!");
                            allGood = false;
                        }
                    }
                    else
                    {
                        generateResponseForInput ("bruteForce: " + arguments[2] + " not found");
                        allGood = false;
                    }
                }

                if (allGood)
                {
                    currentCommand = Commands.BruteForce;
                    bruteForceAction (arguments[1], multiplayer);
                }
            }
        }
        else
        {
            generateResponseForInput ("Command 'bruteForce' not found");
        }
    }

    protected void bruteForceAction (string targetIP, float multiplayer)
    {
        Computer computer = gameState.FindComputerOfIP (targetIP);

        if (computer != null)
        {
            if (!computer.IsPasswordCracted)
            {
                float totalCombinations = Mathf.Pow (SystemHelper.PasswordCharacterTypesSume (computer.Password), (float) computer.Password.Length);
                float percent = computer.Password.Length > 0 ? Random.Range (0.5f, 0.8f) : 1.0f;
                float actualCombinations = totalCombinations * percent;

                float timeToCrack = actualCombinations / multiplayer;
                int hours = ((int)timeToCrack / 3600) < 0 ? -1 * ((int)timeToCrack / 3600) : (int)timeToCrack / 3600;
                int minutes = ((int)(timeToCrack % 3600) / 60) < 0 ? -1 * ((int)(timeToCrack % 3600)) : (int)(timeToCrack % 3600);
                int seconds = ((int)timeToCrack % 60) < 0 ? -1 * ((int)timeToCrack % 60) : (int)timeToCrack % 60;

                generateResponseForInput ("bruteForce: attacking ‘" + targetIP + "‘ with multiplayer: " + multiplayer);
                generateResponseForInput ("Combinations to try: " + totalCombinations);
                generateResponseForInput ($"Estimation time: {hours}h {minutes}min {seconds}s");
                generateResponseForInput ("Attack started");
                PassiveTerminalElement loadingLabel = generateResponseForInputWithPossibleUpdate ("[....................] 0%");
                PassiveTerminalElement traiedCombinationLabel = generateResponseForInputWithPossibleUpdate ("Combination tested: 0");

                StartCoroutine (refreshLayout ());
                StartCoroutine (breakPasswordWithBruteForce (loadingLabel, traiedCombinationLabel, actualCombinations, totalCombinations, multiplayer, computer.SecurityLevel));
            }
            else
            {
                generateResponseForInput ("bruteForce: Computer ‘" + targetIP + "’ has already been hacked");
            }
        }
        else
        {
            generateResponseForInput ("bruteForce: cannot attack ‘" + targetIP + "’: No such computer");
        }

        terminalState = TerminalState.Normal;
        currentCommand = Commands.NotFound;
    }

    protected IEnumerator breakPasswordWithBruteForce (PassiveTerminalElement loadingElement, PassiveTerminalElement testedCombinationLabel, float actualCombination, float totalCombinations, float multiplayer, LevelOfSecurity levelOfSecurity)
    {
        programIsRunning = true;
        playerInputHandler.ChangeIteractibilityOfInputField (false);
        float testesCombination = 0;

        bool seciurityIteraption = false;
        float crackingTime = 0.0f;

        while (programIsRunning && testesCombination < actualCombination && !seciurityIteraption)
        {
            yield return new WaitForSeconds (1f);
            crackingTime += 1f;

            testesCombination += multiplayer;
            float percent = (testesCombination / totalCombinations) * 100;
            int filledDots = Mathf.FloorToInt (percent / 5f);
            string updatedBar = new string ('#', filledDots).PadRight (20, '.');
            
            loadingElement.UpdateText ($"[{updatedBar}] {percent.ToString ("F2")}%");
            testedCombinationLabel.UpdateText ($"Combination tested: {testesCombination}");

            if (levelOfSecurity == LevelOfSecurity.Medium && crackingTime >= 4f)
            {
                seciurityIteraption = true;
            }
            else if (levelOfSecurity == LevelOfSecurity.High && crackingTime >= 2f)
            {
                seciurityIteraption = true;
            }
        }

        if (testesCombination >= actualCombination)
        {
            generateResponseForInput ("bruteForce: password cracked");
            programIsRunning = false;
        }
        else
        {
            if (seciurityIteraption)
            {
                generateResponseForInput ("bruteForce: lost connection with cracking device");
            }
            else
            {
                generateResponseForInput ("bruteForce: cracking was abandoned");
            }
        }

        playerInputHandler.ChangeIteractibilityOfInputField (true);
        playerInputHandler.ActiveInputField ();
    }
}
