using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProgramLogic : MonoBehaviour
{
    protected GameState gameState;
    protected TerminalIterpreter terminalIterpreter;
    protected InputOperator playerInputHandler;

    public delegate void RunProgramHandler (string name, int cpu, int ram, int storage);
    public event RunProgramHandler OnRunProgram;

    public delegate void StopPorgramHandler (string name);
    public event StopPorgramHandler OnStopPorgram;

    public delegate void UpdateProgramHandler (string name, int cpu, int ram, int storage);
    public event UpdateProgramHandler OnUpdateProgram;

    virtual public void Inicialize (TerminalIterpreter terminalIterpreter, InputOperator playerInputHandler)
    {
        gameState = GameState.instance;
        this.terminalIterpreter = terminalIterpreter;
        this.playerInputHandler = playerInputHandler;
    }

    protected void startProgram (string name, int cpu, int ram, int storage)
    {
        OnRunProgram (name, cpu, ram, storage);
    }

    protected void stopProgram (string name)
    {
        OnStopPorgram (name);
    }

    protected void updateProgram (string name, int cpu, int ram, int storage)
    {
        OnUpdateProgram (name, cpu, ram, storage);
    }

    protected void addPasswordToFile (Computer computer)
    {
        TreeNode knownPasswordsFile = terminalIterpreter.TermianlFileSystem.FindNode ("/.config/ssh/known_passwords.txt");

        if (knownPasswordsFile == null)
        {
            knownPasswordsFile = terminalIterpreter.TermianlFileSystem.CreateNode ("/.config/ssh/known_passwords.txt", false);
        }

        knownPasswordsFile.Content += $"{computer.IP}\t{computer.Password}\n";
    }
}
