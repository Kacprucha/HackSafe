using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public enum SshConectionStage
{
    SshConnection,
    SshKeyGeneration,
    SshPassword
}

public class SshLogic : ProgramLogic
{
    protected string user;
    protected Computer computer;

    public delegate void OpenDataBaseHandler (string ip);
    public event OpenDataBaseHandler OnConnectWithdataBase;

    public delegate void CloseDataBaseHandler ();
    public event CloseDataBaseHandler OnDisconnectWithDataBase;

    public void ContinouSSHAction (string[] arguments)
    {
        if (arguments.Length - 1 == 1)
        {
            string[] parts = arguments[1].Split ('@');

            if (parts.Length == 2)
            {
                user = parts[0];
                string ipAddress = parts[1];

                computer = gameState.FindComputerOfIP (ipAddress);

                if (computer == null)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("ssh: cannot connect ‘" + ipAddress + "’: No such computer");
                }
                else if (!computer.CheckIfUserExists(user))
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("ssh: cannot find ‘" + user + "’ user");
                }
                else
                {
                    SshAction (arguments, SshConectionStage.SshConnection);
                }
            }
            else
            {
                terminalIterpreter.GneratePassiveTermialResponse ("ssh: invalid argument format.");
            }
        }
        else if (arguments.Length - 1 < 1)
        {
            terminalIterpreter.GneratePassiveTermialResponse ("ssh: too few arguments!");
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("ssh: too many arguments!");
        }
    }

    public void SshAction (string[] arguments, SshConectionStage stage)
    {
        switch (stage)
        {
            case SshConectionStage.SshConnection:

                terminalIterpreter.GneratePassiveTermialResponse ($"Connecting to {computer.IP}...");

                bool keyExists = false;
                TreeNode knownHostFile = terminalIterpreter.TermianlFileSystem.FindNode ("/.config/ssh/known_hosts.txt");

                if (knownHostFile == null)
                {
                    terminalIterpreter.TermianlFileSystem.CreateNode ("/.config/ssh/known_hosts.txt", false);
                }
                else
                {
                    keyExists = knownHostFile.FindElementInContent (computer.IP);
                }

                if (keyExists)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"{user}@{computer.IP}'s password:");
                    if (computer.IsPasswordCracted)
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("[hackSafe] passing hacked password");
                        connectionMade ();

                        terminalIterpreter.TerminalState = TerminalState.Normal;
                        terminalIterpreter.CurrentCommand = Commands.NotFound;
                    }
                    else
                    {
                        terminalIterpreter.TerminalState = TerminalState.WaitingForPassword;
                        terminalIterpreter.CurrentCommand = Commands.Ssh;

                        playerInputHandler.ChangeColourOfText (true);
                    }
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"The authenticity of host '{computer.IP}' can't be established.");
                    terminalIterpreter.GneratePassiveTermialResponse ($"ED25519 key fingerprint is SHA256:{generateFingerprint (computer.Username)}");
                    terminalIterpreter.GneratePassiveTermialResponse ($"Are you sure you want to continue connecting (yes/no)?");

                    terminalIterpreter.TerminalState = TerminalState.WaitingForConfirmation;
                    terminalIterpreter.CurrentCommand = Commands.Ssh;
                }

                break;

            case SshConectionStage.SshKeyGeneration:

                if (arguments.Length >= 1 && (arguments[0] == "yes" || arguments[0] == "y"))
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"Warning: Permanently added '{computer.IP}' (ED25519) to the list of known hosts.");
                    terminalIterpreter.TermianlFileSystem.FindNode ("/.config/ssh/known_hosts.txt").Content += $"{computer.IP} ED25519:{generateFingerprint (computer.Username)}\n";

                    terminalIterpreter.GneratePassiveTermialResponse ($"{user}@{computer.IP}'s password:");
                    if (computer.IsPasswordCracted)
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("[hackSafe] passing hacked password");
                        connectionMade ();

                        terminalIterpreter.TerminalState = TerminalState.Normal;
                        terminalIterpreter.CurrentCommand = Commands.NotFound;
                    }
                    else
                    {
                        terminalIterpreter.TerminalState = TerminalState.WaitingForPassword;
                        terminalIterpreter.CurrentCommand = Commands.Ssh;

                        playerInputHandler.ChangeColourOfText (true);
                    }
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"Permission denied (publickey).");
                    terminalIterpreter.TerminalState = TerminalState.Normal;
                    terminalIterpreter.CurrentCommand = Commands.NotFound;
                }

                break;

            case SshConectionStage.SshPassword:

                if (arguments.Length >= 1 && computer.Password == arguments[0])
                {
                    connectionMade ();
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"Permission denied, please try again.");
                }

                terminalIterpreter.TerminalState = TerminalState.Normal;
                terminalIterpreter.CurrentCommand = Commands.NotFound;
                playerInputHandler.ChangeColourOfText (false);

                break;
        }
    }

    public void DisConnectFromdataBase ()
    {
        if (OnDisconnectWithDataBase != null)
            OnDisconnectWithDataBase ();
    }

    protected string generateFingerprint (string publicKey)
    {
        string result = "";

        byte[] publicKeyBytes = Encoding.UTF8.GetBytes (publicKey);

        using (SHA256 sha256 = SHA256.Create ())
        {
            byte[] hashBytes = sha256.ComputeHash (publicKeyBytes);

            string base64Hash = Convert.ToBase64String (hashBytes);

            result = $"{base64Hash}";
        }

        return result;
    }

    protected void connectionMade ()
    {
        terminalIterpreter.UpdateFileSystem (computer.IP, computer.FileSystem);
        terminalIterpreter.GneratePassiveTermialResponse ($"Welcome to {computer.IP}!");

        if (computer.IsDataBased && OnConnectWithdataBase != null)
        {
            OnConnectWithdataBase (computer.IP);
        }
    }
}
