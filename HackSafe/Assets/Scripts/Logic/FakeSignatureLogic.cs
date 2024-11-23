using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSignatureLogic : ProgramLogic
{
    public static FakeSignatureLogic Instance { get; private set; }

    public delegate void showingOverlayHandler ();
    public event showingOverlayHandler OnShowOverlay;

    public override void Inicialize (TerminalIterpreter terminalIterpreter, InputOperator playerInputHandler)
    {
        base.Inicialize (terminalIterpreter, playerInputHandler);

        Instance = this;
    }

    public void ContinoueOnFakeSignatureAction (string[] arguments)
    {
        int argumentsAmmount = arguments.Length - 1;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded[TypeOfProgram.fakeSignature])
        {
            if (argumentsAmmount >= 1)
            {
                if (arguments[1] == "-h" || arguments[1] == "--help")
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("Usage: fakeSignature [OPTIONS]");
                    terminalIterpreter.GneratePassiveTermialResponse ("fakeSignature - Open the Fake Signature program for creating notary signatures. Use this tool responsibly and only on systems you have permission to test.");
                    terminalIterpreter.GneratePassiveTermialResponse ("Options:");
                    terminalIterpreter.GneratePassiveTermialResponse ("-h, --help            Show this help message and exit");
                    terminalIterpreter.GneratePassiveTermialResponse (" Notes:");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Atack operate on gui interface.");
                    terminalIterpreter.GneratePassiveTermialResponse ("  - Use at your own risk. Unauthorized use is illegal.");

                    StartCoroutine (terminalIterpreter.RefreshLayout ());
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("fakeSignature: too meny arguments!");
                }
            }
            else
            {
                OnShowOverlay ();
            }
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("Command 'fakeSignature' not found");
        }
    }

    public void StartProgram ()
    {
        startProgram (TypeOfProgram.fakeSignature.ToString (), 20, 10, 0);
    }

    public void UpdateProgram (int cpu, int ram, int disk)
    {
        updateProgram (TypeOfProgram.fakeSignature.ToString (), cpu, ram, disk);
    }

    public void StopProgram ()
    {
        stopProgram (TypeOfProgram.fakeSignature.ToString ());
    }
}
