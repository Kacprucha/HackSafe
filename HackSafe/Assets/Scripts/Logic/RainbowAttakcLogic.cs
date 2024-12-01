using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class RainbowAttakcLogic : ProgramLogic
{
    static List<string> typisalPasswords = new List<string> { "123456", "password", "qwerty", "123456789", "111111", "letmein", "sunshine", "iloveyou", "abc123", "000000", "1q2w3e4r", "admin", "welcome", "monkey", "passw0rd", "master" };
    private float diurationOfCracking = 0.0f;

    public void ContinoueOnRainbowTableAction (string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded[TypeOfProgram.rainbowAttack])
        {
            if (arguments[1] == "-h" || arguments[1] == "--help")
            {
                terminalIterpreter.GneratePassiveTermialResponse ("Usage: rainbowAttack [OPTIONS] <TARGET_IP> <TARGET_HASH>");
                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack - Perform a rainbow table attack to crack a hashed password of a device. Use this tool responsibly and only on systems you have permission to test.");
                terminalIterpreter.GneratePassiveTermialResponse ("Options:");
                terminalIterpreter.GneratePassiveTermialResponse ("-h, --help            Show this help message and exit");
                terminalIterpreter.GneratePassiveTermialResponse ("Example:");
                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack 192.168.1.5 d7a8fbb307d7809469ca9ab8990a6da5");
                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack 172.26.1.16 d7a8fbb307d7809469ca9ab8990a");
                terminalIterpreter.GneratePassiveTermialResponse (" Notes:");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure the node with ip exists and contains potential passwords.");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure the hash is a password for device of given ip.");
                terminalIterpreter.GneratePassiveTermialResponse ("  - Use at your own risk. Unauthorized use is illegal.");

                StartCoroutine (terminalIterpreter.RefreshLayout ());
            }
            else
            {
                if (argumentsAmmount == 2)
                {
                    terminalIterpreter.CurrentCommand = Commands.RainbowAttack;
                    rainbowAttackAction (arguments[1], arguments[2]);
                }
                else if (argumentsAmmount > 2)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: too meny arguments!");
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: too few arguments!");
                }
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'rainbowAttack' not found");
        }
    }

    protected void rainbowAttackAction (string targetIP, string hashedPassword)
    {
        Computer computer = gameState.FindComputerOfIP (targetIP);

        if (computer != null)
        {
            if (!computer.IsPasswordCracted)
            {
                startProgram (TypeOfProgram.rainbowAttack.ToString (), 0, 10, 30);

                terminalIterpreter.GneratePassiveTermialResponse ("Starting rainbow table attack on target ‘" + targetIP + "‘");
                PassiveTerminalElement loadingLabel = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Loading precomputed rainbow table...");

                StartCoroutine (terminalIterpreter.RefreshLayout ());
                StartCoroutine (breakPasswordWithRainbowAttack (loadingLabel, computer, hashedPassword));
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: Computer ‘" + targetIP + "’ has already been hacked");
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: cannot attack ‘" + targetIP + "’: No such computer");
        }

        terminalIterpreter.TerminalState = TerminalState.Normal;
        terminalIterpreter.CurrentCommand = Commands.NotFound;
    }

    protected IEnumerator breakPasswordWithRainbowAttack (PassiveTerminalElement loadingElement, Computer computer, string hashedPassword)
    {
        yield return new WaitForSeconds (2.3f);
        loadingElement.UpdateText ("Loading precomputed rainbow table... [OK]");
        updateProgram (TypeOfProgram.rainbowAttack.ToString (), 30, 20, 10);

        terminalIterpreter.GneratePassiveTermialResponse ("Table size: 500,000 chains | Hash type: SHA-256");
        terminalIterpreter.GneratePassiveTermialResponse ("Searching for matching hash in rainbow table...");
        terminalIterpreter.GneratePassiveTermialResponse ("Hash Value            | Status");
        terminalIterpreter.GneratePassiveTermialResponse ("------------------------------");
        StartCoroutine (terminalIterpreter.RefreshLayout ());

        terminalIterpreter.ProgramIsRunning = true;
        playerInputHandler.ChangeIteractibilityOfInputField (false);

        LevelOfSecurity levelOfSecurity = computer.SecurityLevel;

        string realHashedPassword = computeSHA256Hash (computer.Password);
        UnityEngine.Debug.Log (realHashedPassword);

        setAttackDuration (levelOfSecurity);

        PassiveTerminalElement curentTryedPassword = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("");

        Stopwatch stopwatch = new Stopwatch ();
        stopwatch.Start ();

        bool firstIteration = true;

        while (terminalIterpreter.ProgramIsRunning && stopwatch.Elapsed.TotalSeconds < diurationOfCracking)
        {
            int position = Random.Range (0, typisalPasswords.Count);
            string password = typisalPasswords[position];
            password = computeSHA256Hash (password);

            password = password.Substring (0, 15);

            curentTryedPassword.UpdateText ("Hash: " + password + " [No Match]   ");

            if (firstIteration)
            {
                StartCoroutine (terminalIterpreter.RefreshLayout ());
                firstIteration = false;
            }

            yield return new WaitForEndOfFrame ();
        }

        stopwatch.Stop ();

        if (stopwatch.Elapsed.TotalSeconds >= diurationOfCracking && levelOfSecurity != LevelOfSecurity.None)
        {
            if (hashedPassword.Length >= 16)
                curentTryedPassword.UpdateText ($"Hash: {hashedPassword.Substring (0, 15) } [Match Found]");
            else
                curentTryedPassword.UpdateText ($"Hash: {hashedPassword} [Match Found]");

            terminalIterpreter.GneratePassiveTermialResponse ("[Match Found] Chain endpoint located. Retrieving password chain...");
            terminalIterpreter.GneratePassiveTermialResponse ("Recomputing reduction chain from start password...");
            terminalIterpreter.GneratePassiveTermialResponse ("Chain Start  | Hash             | Reduction");
            terminalIterpreter.GneratePassiveTermialResponse ("-------------------------------------------");
            StartCoroutine (terminalIterpreter.RefreshLayout ());

            curentTryedPassword = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("");

            if (hashedPassword != realHashedPassword)
                levelOfSecurity = LevelOfSecurity.None;

            setAttackDuration (levelOfSecurity);

            stopwatch = new Stopwatch ();
            stopwatch.Start ();

            firstIteration = true;

            while (terminalIterpreter.ProgramIsRunning && stopwatch.Elapsed.TotalSeconds < diurationOfCracking)
            {
                int positionS = Random.Range (0, typisalPasswords.Count);
                int positionE = Random.Range (0, typisalPasswords.Count);
                string passwordS = typisalPasswords[positionS];
                string passwordE = typisalPasswords[positionE];
                string hash = computeSHA256Hash (passwordS);

                while (passwordS.Length < 9)
                {
                    passwordS += " ";
                }

                hash = hash.Substring (0, 15);

                while (passwordE.Length < 9)
                {
                    passwordE += " ";
                }

                curentTryedPassword.UpdateText ($"{passwordS} -> {hash} -> {passwordE}");

                if (firstIteration)
                {
                    StartCoroutine (terminalIterpreter.RefreshLayout ());
                    firstIteration = false;
                }

                yield return new WaitForEndOfFrame ();
            }

            stopwatch.Stop ();

            if (stopwatch.Elapsed.TotalSeconds >= diurationOfCracking && levelOfSecurity != LevelOfSecurity.None)
            {
                curentTryedPassword.UpdateText ($"{computer.Password} -> {hashedPassword.Substring(0,15)} -> Match!");

                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: password cracked");

                addPasswordToFile (computer);

                gameState.FindComputerOfIP (computer.IP).IsPasswordCracted = true;
            }
            else
            {
                if (levelOfSecurity == LevelOfSecurity.None && levelOfSecurity != computer.SecurityLevel)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: time out, cracking took to much time.");
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: cracking was abandoned");
                }
            }
        }
        else
        {
            if (computer.SecurityLevel == LevelOfSecurity.None && terminalIterpreter.ProgramIsRunning)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: time out, cracking took to much time.");
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("rainbowAttack: cracking was abandoned");
            }
        }

        updateProgram (TypeOfProgram.rainbowAttack.ToString (), 5, 5, 15);
        PassiveTerminalElement delateLabel = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Delating precomputed rainbow table...");
        yield return new WaitForSeconds (1f);
        delateLabel.UpdateText ("Delating precomputed rainbow table... [OK]");

        stopProgram (TypeOfProgram.rainbowAttack.ToString ());

        playerInputHandler.ChangeIteractibilityOfInputField (true);
        playerInputHandler.ActiveInputField ();

        terminalIterpreter.ProgramIsRunning = false;
    }

    protected void setAttackDuration (LevelOfSecurity levelOfSecurity)
    {
        switch (levelOfSecurity)
        {
            case LevelOfSecurity.Low:
                diurationOfCracking = 5f / 2;
                break;

            case LevelOfSecurity.Medium:
                diurationOfCracking = 10f / 2;
                break;

            case LevelOfSecurity.High:
                diurationOfCracking = 15f / 2;
                break;

            case LevelOfSecurity.None:
                diurationOfCracking = 300f;
                break;

            default:
                diurationOfCracking = 60f;
                break;
        }
    }

    protected string computeSHA256Hash (string password)
    {
        byte[] bytes = Encoding.UTF8.GetBytes (password);

        using (SHA256 sha256 = SHA256.Create ())
        {
            byte[] hashBytes = sha256.ComputeHash (bytes);

            StringBuilder hashString = new StringBuilder ();
            foreach (byte b in hashBytes)
            {
                hashString.Append (b.ToString ("x2"));
            }

            return hashString.ToString ();
        }
    }
}
