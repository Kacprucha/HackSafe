using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class DicionaryAttackLogic : ProgramLogic
{
    static List<string> typisalPasswords = new List<string> { "123456", "password", "qwerty", "123456789", "111111", "letmein", "sunshine", "iloveyou", "abc123", "000000", "1q2w3e4r", "admin", "welcome", "monkey", "passw0rd", "master"};
    private float diurationOfCracking = 0.0f;

    public void ContinoueOnDictionaryAction (string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded[TypeOfProgram.dictionaryAttack])
        {
            if (arguments[1] == "-h" || arguments[1] == "--help")
            {
                terminalIterpreter.GneratePassiveTermialResponse ("Usage: dictionaryAttack [OPTIONS] <TARGET_IP>");
                terminalIterpreter.GneratePassiveTermialResponse ("Dictionary attack tool for penetration testing. Use this tool responsibly and only on systems you have permission to test.");
                terminalIterpreter.GneratePassiveTermialResponse ("Options:");
                terminalIterpreter.GneratePassiveTermialResponse ("-h, --help            Show this help message and exit");
                terminalIterpreter.GneratePassiveTermialResponse ("Example:");
                terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack 192.168.1.5");
                terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack 192.168.1.100");
                terminalIterpreter.GneratePassiveTermialResponse (" Notes:");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure the node with ip exists and contains potential passwords.");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Dictionary attack attacks can not work with unic passwords.");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Use at your own risk. Unauthorized use is illegal.");

                StartCoroutine (terminalIterpreter.RefreshLayout ());
            }
            else
            {
                if (argumentsAmmount == 1)
                {
                    terminalIterpreter.CurrentCommand = Commands.DictionaryAttack;
                    dictionaryAttackAction (arguments[1]);
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack: too meny arguments!");
                }
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'dictionaryAttack' not found");
        }
    }

    protected void dictionaryAttackAction (string targetIP)
    {
        Computer computer = gameState.FindComputerOfIP (targetIP);

        if (computer != null)
        {
            if (!computer.IsPasswordCracted)
            {
                startProgram (TypeOfProgram.dictionaryAttack.ToString (), 0, 10, 60);

                terminalIterpreter.GneratePassiveTermialResponse ("Starting dictionary attack on target ‘" + targetIP + "‘");
                PassiveTerminalElement loadingLabel = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Loading dictionary file...");

                StartCoroutine (terminalIterpreter.RefreshLayout ());
                StartCoroutine (breakPasswordWithDictionaryAttack (loadingLabel, computer));
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack: Computer ‘" + targetIP + "’ has already been hacked");
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack: cannot attack ‘" + targetIP + "’: No such computer");
        }

        terminalIterpreter.TerminalState = TerminalState.Normal;
        terminalIterpreter.CurrentCommand = Commands.NotFound;
    }

    protected IEnumerator breakPasswordWithDictionaryAttack (PassiveTerminalElement loadingElement, Computer computer)
    {
        yield return new WaitForSeconds (2.3f);
        loadingElement.UpdateText ("Loading dictionary file... [OK]");
        updateProgram (TypeOfProgram.dictionaryAttack.ToString (), 30, 20, 10);

        terminalIterpreter.ProgramIsRunning = true;
        playerInputHandler.ChangeIteractibilityOfInputField (false);

        setAttackDuration (computer.SecurityLevel);

        PassiveTerminalElement curentTryedPassword = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("");

        Stopwatch stopwatch = new Stopwatch ();
        stopwatch.Start ();

        while (terminalIterpreter.ProgramIsRunning && stopwatch.Elapsed.TotalSeconds < diurationOfCracking)
        {
            int position = Random.Range (0, typisalPasswords.Count);
            string password = typisalPasswords[position];

            while (password.Length < 9)
            {
                password += " ";
            }

            curentTryedPassword.UpdateText ("Attempting password: " + password + " [FAILED]");
            yield return new WaitForEndOfFrame ();
        }

        stopwatch.Stop ();

        if (stopwatch.Elapsed.Seconds >= diurationOfCracking)
        {
            curentTryedPassword.UpdateText ("Attempting password: " + computer.Password + " [SUCCESS]");
            terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack: password cracked");

            addPasswordToFile (computer);

            gameState.FindComputerOfIP (computer.IP).IsPasswordCracted = true;
            terminalIterpreter.ProgramIsRunning = false;
        }
        else
        {
            if (computer.SecurityLevel == LevelOfSecurity.High)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack: lost connection with cracking device");
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("dictionaryAttack: cracking was abandoned");
            }
        }

        updateProgram (TypeOfProgram.dictionaryAttack.ToString (), 5, 5, 30);
        PassiveTerminalElement delateLabel = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Delating dictionary file...");
        yield return new WaitForSeconds (1f);
        delateLabel.UpdateText ("Delating dictionary file... [OK]");

        stopProgram (TypeOfProgram.dictionaryAttack.ToString ());

        playerInputHandler.ChangeIteractibilityOfInputField (true);
        playerInputHandler.ActiveInputField ();
    }

    protected void setAttackDuration (LevelOfSecurity levelOfSecurity)
    {
        switch (levelOfSecurity)
        {
            case LevelOfSecurity.Low:
                diurationOfCracking = 5f; 
                break;

            case LevelOfSecurity.Medium:
                diurationOfCracking = 28f;  
                break;

            case LevelOfSecurity.High:
            case LevelOfSecurity.None:
                diurationOfCracking = 300f; 
                break;

            default:
                diurationOfCracking = 60f;  
                break;
        }
    }
}
