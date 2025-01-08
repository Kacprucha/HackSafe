using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AptLogic : ProgramLogic
{
    protected List<TypeOfProgram> programsToInstall = new List<TypeOfProgram> ();

    public void ContinoueOnAptAction (bool sudoUsed, string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (argumentsAmmount > 0)
        {
            if (arguments[1] == "install")
            {
                if (argumentsAmmount - 1 > 0)
                {
                    terminalIterpreter.CurrentCommand = Commands.Install;
                    continoueOnProtectedAction (sudoUsed, "apt install");

                    for (int i = 2; i < arguments.Length; i++)
                    {
                        switch (arguments[i])
                        {
                            case "bruteForce":
                                programsToInstall.Add (TypeOfProgram.brutForse);

                                break;

                            case "rainbowAttack":
                                programsToInstall.Add (TypeOfProgram.rainbowAttack);

                                break;

                            case "dictionaryAttack":
                                programsToInstall.Add (TypeOfProgram.dictionaryAttack);

                                break;

                            case "manInTheMiddle":
                                programsToInstall.Add (TypeOfProgram.manInTheMiddle);

                                break;

                            case "fakeSignature":
                                programsToInstall.Add (TypeOfProgram.fakeSignature);

                                break;
                        }
                    }
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("apt install: too few arguments!");
                }
            }
            else if (arguments[1] == "update")
            {
                if (argumentsAmmount - 1 == 0)
                {
                    terminalIterpreter.CurrentCommand = Commands.Update;
                    continoueOnProtectedAction (sudoUsed, "apt update");
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("apt update: too many arguments!");
                }
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("apt: " + arguments[1] + " not found");
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("apt: too few arguments!");
        }
    }

    protected void continoueOnProtectedAction (bool ifSudoUsed, string nameOfAction)
    {
        if (ifSudoUsed)
        {
            terminalIterpreter.GneratePassiveTermialResponse ("[sudo] password for " + gameState.GetPlayerInfo ().PlayerComputer.Username + ":");
            terminalIterpreter.TerminalState = TerminalState.WaitingForSudoPassword;

            playerInputHandler.ChangeColourOfText (true);
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse (nameOfAction + ": permission denied");
            terminalIterpreter.CurrentCommand = Commands.NotFound;
        }
    }

    public void ContinoueOnUpdateAction (string[] arguments)
    {
        Computer computer = terminalIterpreter.TerminalIP == gameState.GetPlayerInfo ().PlayerComputer.IP ? gameState.GetPlayerInfo ().PlayerComputer : gameState.FindComputerOfIP (terminalIterpreter.TerminalIP);

        if (arguments.Length == 1 && computer.CheckIfGivenPasswordIsCorrect (arguments[0]))
        {
            updateAction ();
        }
        else if (terminalIterpreter.TerminalState == TerminalState.WaitingForSudoPassword)
        {
            terminalIterpreter.GneratePassiveTermialResponse ("update: permission denied");
            playerInputHandler.ChangeColourOfText (false);
            terminalIterpreter.TerminalState = TerminalState.Normal;
            terminalIterpreter.CurrentCommand = Commands.NotFound;
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'update' not found, did you mean command 'apt update'");
        }
    }

    protected void updateAction ()
    {
        playerInputHandler.ChangeIteractibilityOfInputField (false);
        StartCoroutine (TerminalMenager.GenerateTerminalResponseForUpdate (terminalIterpreter));
        terminalIterpreter.TerminalState = TerminalState.Normal;
        terminalIterpreter.CurrentCommand = Commands.NotFound;
    }

    public void ContinoueOnInstallAction (string[] arguments)
    {
        Computer computer = terminalIterpreter.TerminalIP == gameState.GetPlayerInfo ().PlayerComputer.IP ? gameState.GetPlayerInfo ().PlayerComputer : gameState.FindComputerOfIP (terminalIterpreter.TerminalIP);

        if (computer.CheckIfGivenPasswordIsCorrect (arguments[0]))
        {
            bool allProgramsAllowed = false;

            if (programsToInstall.Count > 0)
            {
                foreach (TypeOfProgram program in programsToInstall)
                {
                    if (TerminalMenager.CheckIfPlayerCanDownloadProgram (program))
                    {
                        allProgramsAllowed = true;
                    }
                    else
                    {
                        playerInputHandler.ChangeColourOfText (false);
                        terminalIterpreter.GneratePassiveTermialResponse ("apt install: you don't have access to install " + program + " or this programm does not exist");

                        programsToInstall = new List<TypeOfProgram> ();
                        allProgramsAllowed = false;

                        terminalIterpreter.TerminalState = TerminalState.Normal;
                        terminalIterpreter.CurrentCommand = Commands.NotFound;

                        break;
                    }
                }
            }
            else
            {
                playerInputHandler.ChangeColourOfText (false);
                terminalIterpreter.GneratePassiveTermialResponse ("apt install: one or more programms you want to install does not exist");

                programsToInstall = new List<TypeOfProgram> ();
                allProgramsAllowed = false;

                terminalIterpreter.TerminalState = TerminalState.Normal;
                terminalIterpreter.CurrentCommand = Commands.NotFound;
            }

            if (allProgramsAllowed)
            {
                List<TypeOfProgram> tempPrograms = new List<TypeOfProgram> (programsToInstall);
                foreach (TypeOfProgram program in tempPrograms)
                {
                    if (TerminalMenager.CheckIfPlayerDownloadedProgram (program))
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("apt install: " + program + " is already installed");
                        programsToInstall.Remove (program);
                    }
                }

                if (programsToInstall.Count > 0)
                {
                    installAction (programsToInstall, true);
                }
                else
                {
                    playerInputHandler.ChangeColourOfText (false);
                    terminalIterpreter.TerminalState = TerminalState.Normal;
                    terminalIterpreter.CurrentCommand = Commands.NotFound;
                }
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'install' not found, did you mean command 'apt install'");
        }
    }

    public void ContinoueOnInstallConfirmAction (string[] arguments)
    {
        if (arguments.Length == 1 && TerminalMenager.CheckIfResponseIsYes (arguments[0]))
        {
            installAction (programsToInstall, false);
            programsToInstall = new List<TypeOfProgram> ();
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("apt install: instalation aborted");
            terminalIterpreter.TerminalState = TerminalState.Normal;
            terminalIterpreter.CurrentCommand = Commands.NotFound;

            stopProgram ("apt");
        }
    }

    protected void installAction (List<TypeOfProgram> programsToInstall, bool beforeAproveFaze)
    {
        if (beforeAproveFaze)
        {
            playerInputHandler.ChangeIteractibilityOfInputField (false);
            StartCoroutine (TerminalMenager.GenerateTermianlResponseForInstall (terminalIterpreter, programsToInstall, beforeAproveFaze));
            terminalIterpreter.TerminalState = TerminalState.WaitingForConfirmation;
        }
        else
        {
            startProgram ("apt", 0, 5, programsToInstall.Count * 10);

            playerInputHandler.ChangeIteractibilityOfInputField (false);
            StartCoroutine (TerminalMenager.GenerateTermianlResponseForInstall (terminalIterpreter, programsToInstall, beforeAproveFaze));

            foreach (TypeOfProgram program in programsToInstall)
            {
                gameState.GetPlayerInfo ().ProgramesDownloaded[program] = true;
            }

            terminalIterpreter.TerminalState = TerminalState.Normal;
            terminalIterpreter.CurrentCommand = Commands.NotFound;
        }
    }

    public void StopProgram ()
    {
        stopProgram ("apt");
    }
}
