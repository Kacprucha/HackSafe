using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManInTheMiddleLogic : ProgramLogic
{
    public static ManInTheMiddleLogic Instance { get; private set; }
    public Task AsociatedTask;

    public delegate void showingOverlayHandler (Computer compA, Computer compB);
    public event showingOverlayHandler OnShowOverlay;

    public override void Inicialize (TerminalIterpreter terminalIterpreter, InputOperator playerInputHandler)
    {
        base.Inicialize (terminalIterpreter, playerInputHandler);

        Instance = this;
    }

    public void ContinoueOnManInTheMiddleAction (string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded[TypeOfProgram.manInTheMiddle])
        {
            if (argumentsAmmount >= 1)
            {
                if (arguments[1] == "-h" || arguments[1] == "--help")
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("Usage: manInTheMiddle [OPTIONS] <SENDER_IP> <RECIVER_IP>");
                    terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle - Launch a man-in-the-middle attack to intercept data between two devices. Use this tool responsibly and only on systems you have permission to test.");
                    terminalIterpreter.GneratePassiveTermialResponse ("Options:");
                    terminalIterpreter.GneratePassiveTermialResponse ("-h, --help            Show this help message and exit");
                    terminalIterpreter.GneratePassiveTermialResponse ("Example:");
                    terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle 192.168.1.5 192.168.1.10");
                    terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle 172.168.20.130 192.168.80.1");
                    terminalIterpreter.GneratePassiveTermialResponse (" Notes:");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure the nodes with ips exists in network.");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure the nodes with ips are not the same.");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Ensure that you have acces to the nodes.");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Atack operate on gui interface.");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Use at your own risk. Unauthorized use is illegal.");

                    StartCoroutine (terminalIterpreter.RefreshLayout ());
                }
                else
                {
                    if (argumentsAmmount == 2)
                    {
                        Computer compA = gameState.FindComputerOfIP (arguments[1]);
                        Computer compB = gameState.FindComputerOfIP (arguments[2]);

                        if (compA != null && compB != null)
                        {
                            if (compA.IP == compB.IP)
                            {
                                terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle: you have to put 2 different ips!");
                            }
                            else if (!compA.IsPasswordCracted && !compB.IsPasswordCracted)
                            {
                                terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle: one of the computers is not hacked!");
                            }
                            else
                            {
                                OnShowOverlay (compA, compB);
                            }
                        }
                        else
                        {
                            terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle: one of the computers not found!");
                        }

                    }
                    else
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle: too meny arguments!");
                    }
                }
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("manInTheMiddle: too few arguments!");
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'manInTheMiddle' not found");
        }
    }

    public bool CheckIfKeyCorrect (Computer comp, Key key, bool isSuppsotToBePublic)
    {
        bool result = false;

        Computer playerComputer = gameState.GetPlayerInfo ().PlayerComputer;

        if (key.KeyPublic == isSuppsotToBePublic && key.AssociatedComputerIP == comp.IP && key.CratorComputerIP == playerComputer.IP)
        {
            result = true;
        }

        return result;
    }

    public bool CheckIsFormulaCorrect (Computer comp, Key publicK, Key privateK)
    {
        bool result = false;

        Computer playerComputer = gameState.GetPlayerInfo ().PlayerComputer;

        if (publicK.CratorComputerIP == comp.IP && privateK.CratorComputerIP == playerComputer.IP && publicK.AssociatedComputerIP == comp.IP && privateK.AssociatedComputerIP == comp.IP)
        {
            result = true;
        }

        return result;
    }

    public void MarkTaKaskAsDone ()
    {
        AsociatedTask.SetTaskDone ();
        gameState.MareTaskAsDone (gameState.ActiveQuest.ID, AsociatedTask.ID);
    }

    public void StartProgram ()
    {
        startProgram (TypeOfProgram.manInTheMiddle.ToString (), 30, 20, 0);
    }

    public IEnumerator UpdateProgram (bool isFileDowloading)
    {
        if (isFileDowloading)
        {
            updateProgram (TypeOfProgram.manInTheMiddle.ToString (), 10, 20, 30);
            yield return new WaitForSeconds (2.3f);
            updateProgram (TypeOfProgram.manInTheMiddle.ToString (), 10, 10, 0);

        }
        else
        {
            updateProgram (TypeOfProgram.manInTheMiddle.ToString (), 20, 30, 0);
        }
    }

    public void StopProgram ()
    {
        stopProgram (TypeOfProgram.manInTheMiddle.ToString ());
    }
}
