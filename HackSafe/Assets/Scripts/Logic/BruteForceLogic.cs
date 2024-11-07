using System.Collections;
using UnityEngine;

public class BruteForceLogic : ProgramLogic
{
    public void ContinoueOnBruteForceAction (string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded[TypeOfProgram.brutForse])
        {
            if (argumentsAmmount == 0)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: too few arguments!");
            }
            else if (arguments[1] == "-h" || arguments[1] == "--help")
            {
                terminalIterpreter.GneratePassiveTermialResponse ("Usage: bruteforce <TARGET_IP> [OPTIONS]");
                terminalIterpreter.GneratePassiveTermialResponse ("Brute force attack tool for penetration testing. Use this tool responsibly and only on systems you have permission to test.");
                terminalIterpreter.GneratePassiveTermialResponse ("Options:");
                terminalIterpreter.GneratePassiveTermialResponse ("-h, --help            Show this help message and exit");
                terminalIterpreter.GneratePassiveTermialResponse ("-m, --multiplier      Set the multiplier for the attack");
                terminalIterpreter.GneratePassiveTermialResponse ("Example:");
                terminalIterpreter.GneratePassiveTermialResponse ("bruteforce 192.168.1.5");
                terminalIterpreter.GneratePassiveTermialResponse ("bruteforce 192.168.1.100 -m 5");
                terminalIterpreter.GneratePassiveTermialResponse (" Notes:");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure the node with ip exists and contains potential passwords.");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Brute force attacks can take significant time, especially with complex passwords.");
                terminalIterpreter.GneratePassiveTermialResponse ("  - The multiplier option is used to increase the number of attempts per second. The default is 1. It can't be more then 300");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Use at your own risk. Unauthorized use is illegal.");

                StartCoroutine (terminalIterpreter.RefreshLayout ());
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
                                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: multiplier is to big!");
                                allGood = false;
                            }
                            else if (multiplayer < 1)
                            {
                                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: multiplier has to be at least 1!");
                                allGood = false;
                            }
                        }
                        else
                        {
                            terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: too few arguments!");
                            allGood = false;
                        }
                    }
                    else
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: " + arguments[2] + " not found");
                        allGood = false;
                    }
                }

                if (allGood)
                {
                    terminalIterpreter.CurrentCommand = Commands.BruteForce;
                    bruteForceAction (arguments[1], multiplayer);
                }
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'bruteForce' not found");
        }
    }

    protected void bruteForceAction (string targetIP, float multiplayer)
    {
        Computer computer = gameState.FindComputerOfIP (targetIP);

        if (computer != null)
        {
            if (!computer.IsPasswordCracted)
            {
                float totalCombinations = Mathf.Pow (SystemHelper.PasswordCharacterTypesSume (computer.Password), (float)computer.Password.Length);
                float percent = computer.Password.Length > 0 ? Random.Range (0.5f, 0.8f) : 1.0f;
                float actualCombinations = totalCombinations * percent;

                float timeToCrack = actualCombinations / multiplayer;
                int hours = ((int)timeToCrack / 3600) < 0 ? -1 * ((int)timeToCrack / 3600) : (int)timeToCrack / 3600;
                int minutes = ((int)(timeToCrack % 3600) / 60) < 0 ? -1 * ((int)(timeToCrack % 3600) / 60) : (int)((timeToCrack % 3600) / 60);
                int seconds = ((int)timeToCrack % 60) < 0 ? -1 * ((int)timeToCrack % 60) : (int)timeToCrack % 60;

                startProgram (TypeOfProgram.brutForse.ToString (), 60, 40, 0);

                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: attacking ‘" + targetIP + "‘ with multiplayer: " + multiplayer);
                terminalIterpreter.GneratePassiveTermialResponse ("Combinations to try: " + totalCombinations);
                terminalIterpreter.GneratePassiveTermialResponse ($"Estimation time: {hours}h {minutes}min {seconds}s");
                terminalIterpreter.GneratePassiveTermialResponse ("Attack started");
                PassiveTerminalElement loadingLabel = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("[....................] 0%");
                PassiveTerminalElement traiedCombinationLabel = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Combination tested: 0");

                StartCoroutine (terminalIterpreter.RefreshLayout ());
                StartCoroutine (breakPasswordWithBruteForce (loadingLabel, traiedCombinationLabel, actualCombinations, totalCombinations, multiplayer, computer.IP, computer.SecurityLevel));
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: Computer ‘" + targetIP + "’ has already been hacked");
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: cannot attack ‘" + targetIP + "’: No such computer");
        }

        terminalIterpreter.TerminalState = TerminalState.Normal;
        terminalIterpreter.CurrentCommand = Commands.NotFound;
    }

    protected IEnumerator breakPasswordWithBruteForce (PassiveTerminalElement loadingElement, PassiveTerminalElement testedCombinationLabel, float actualCombination, float totalCombinations, float multiplayer, string ip, LevelOfSecurity levelOfSecurity)
    {
        yield return new WaitForSeconds (1f);
        updateProgram (TypeOfProgram.brutForse.ToString (), 80, 20, 0);

        terminalIterpreter.ProgramIsRunning = true;
        playerInputHandler.ChangeIteractibilityOfInputField (false);
        float testesCombination = 0;

        bool seciurityIteraption = false;
        float crackingTime = 0.0f;

        while (terminalIterpreter.ProgramIsRunning && testesCombination < actualCombination && !seciurityIteraption)
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
            terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: password cracked");

            addPasswordToFile (gameState.FindComputerOfIP (ip));

            gameState.FindComputerOfIP (ip).IsPasswordCracted = true;
            terminalIterpreter.ProgramIsRunning = false;
        }
        else
        {
            if (seciurityIteraption)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: lost connection with cracking device");
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("bruteForce: cracking was abandoned");
            }
        }

        stopProgram (TypeOfProgram.brutForse.ToString ());

        playerInputHandler.ChangeIteractibilityOfInputField (true);
        playerInputHandler.ActiveInputField ();
    }
}
