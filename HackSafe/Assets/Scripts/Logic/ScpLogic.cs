using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;

public class ScpLogic : ProgramLogic
{
    protected string user;
    protected Computer computer;

    protected bool fromLocalToRemote = false;
    string pathToCoppy = "";
    string pathToPaste = "";

    public delegate void OpenDataBaseHandler (string ip);
    public event OpenDataBaseHandler OnConnectWithdataBase;

    public delegate void CloseDataBaseHandler ();
    public event CloseDataBaseHandler OnDisconnectWithDataBase;

    public void ContinouOnScpAction (string[] arguments)
    {
        if (arguments.Length - 1 == 2)
        {
            string[] parts = arguments[1].Split ('@');

            string ipAddress = "";

            bool allCorrect = true;

            if (parts.Length == 2)
            {
                fromLocalToRemote = false;

                string[] temp = parts[1].Split (':');
                if (temp.Length == 2)
                {
                    user = parts[0];
                    ipAddress = temp[0];
                    pathToCoppy = temp[1];
                    pathToPaste = arguments[2];
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("scp: invalid argument format.");
                    allCorrect = false;
                }
            }
            else
            {
                fromLocalToRemote = true;
                pathToCoppy = arguments[1];

                parts = arguments[2].Split ('@');
                if (parts.Length == 2)
                {
                    user = parts[0];

                    parts = parts[1].Split (':');
                    if (parts.Length == 2)
                    {
                        ipAddress = parts[0];
                        pathToPaste = parts[1];
                    }
                    else
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("scp: invalid argument format.");
                        allCorrect = false;
                    }
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("scp: invalid argument format.");
                    allCorrect = false;
                }

            }

            if (allCorrect)
            {
                computer = gameState.FindComputerOfIP (ipAddress);

                if (ipAddress == gameState.GetPlayerInfo ().PlayerComputer.IP)
                    computer = gameState.GetPlayerInfo ().PlayerComputer;

                if (computer == null)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot find ‘" + ipAddress + "’: No such computer");
                }
                else if (!computer.CheckIfUserExists (user))
                {
                    terminalIterpreter.GneratePassiveTermialResponse ("ssp: cannot find ‘" + user + "’ user");
                }
                else if (fromLocalToRemote)
                {
                    if (!pathToCoppy.StartsWith ("/"))
                    {
                        pathToCoppy = terminalIterpreter.TermianlFileSystem.GetPathOfCurrentDirectory () + "/" + pathToCoppy;
                    }

                    TreeNode elementToCopy = terminalIterpreter.TermianlFileSystem.FindNode (pathToCoppy);

                    if (elementToCopy == null)
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot find ‘" + pathToCoppy + "’: No such file or directory");
                    }
                    else if (elementToCopy.IsDirectory)
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot copy directory");
                    }
                    else
                    {
                        ScpAction (arguments, SshConectionStage.SshConnection);
                    }
                }
                else
                {
                    if (!pathToPaste.StartsWith ("/"))
                    {
                        pathToPaste = terminalIterpreter.TermianlFileSystem.GetPathOfCurrentDirectory () + "/" + pathToPaste;
                    }

                    TreeNode elementToPaste = terminalIterpreter.TermianlFileSystem.FindNode (pathToPaste);

                    if (elementToPaste == null)
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot find ‘" + pathToPaste + "’: No such file or directory");
                    }
                    else if (!elementToPaste.IsDirectory)
                    {
                        terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot paste to this directory");
                    }
                    else
                    {
                        ScpAction (arguments, SshConectionStage.SshConnection);
                    }
                }
            }
        }
        else if (arguments.Length - 1 < 2)
        {
            terminalIterpreter.GneratePassiveTermialResponse ("scp: too few arguments!");
        }
        else
        {
            terminalIterpreter.GneratePassiveTermialResponse ("scp: too many arguments!");
        }
    }

    public void ScpAction (string[] arguments, SshConectionStage stage)
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
                        terminalIterpreter.CurrentCommand = Commands.Scp;

                        playerInputHandler.ChangeColourOfText (true);
                    }
                }
                else
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"The authenticity of host '{computer.IP}' can't be established.");
                    terminalIterpreter.GneratePassiveTermialResponse ($"ED25519 key fingerprint is SHA256:{generateFingerprint (computer.Username)}");
                    terminalIterpreter.GneratePassiveTermialResponse ($"Are you sure you want to continue connecting (yes/no)?");

                    terminalIterpreter.TerminalState = TerminalState.WaitingForConfirmation;
                    terminalIterpreter.CurrentCommand = Commands.Scp;
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
                        terminalIterpreter.CurrentCommand = Commands.Scp;

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
        if (fromLocalToRemote)
        {
            TreeNode directoryToPaste = computer.FileSystem.FindNode (pathToPaste);
            
            if (directoryToPaste != null && !directoryToPaste.IsDirectory)
            {
                terminalIterpreter.GneratePassiveTermialResponse ($"scp: cannot paste to this directory");
            }
            else
            {
                TreeNode elementToCopy = terminalIterpreter.TermianlFileSystem.FindNode (pathToCoppy);
                string filePath = pathToPaste + "/" + elementToCopy.Name;

                if (directoryToPaste == null)
                {
                    computer.FileSystem.CreateNode (pathToPaste, true);
                    directoryToPaste = computer.FileSystem.FindNode (pathToPaste);
                }
                else if (computer.FileSystem.FindNode (filePath) != null)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"scp: {elementToCopy.Name} already exists in {pathToPaste}");
                }
                else
                {
                    computer.FileSystem.CreateNode (filePath, false).Content = elementToCopy.Content;
                    terminalIterpreter.GneratePassiveTermialResponse ("scp: coping finish successfully!");
                }
            }
        }
        else
        {
            TreeNode fileToCopy = computer.FileSystem.FindNode (pathToCoppy);

            if (fileToCopy == null)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot find ‘" + pathToCoppy + "’: No such file or directory");
            }
            else if (fileToCopy.IsDirectory)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("scp: cannot copy directory");
            }
            else
            {
                TreeNode directoryToPaste = terminalIterpreter.TermianlFileSystem.FindNode (pathToPaste);

                if (directoryToPaste == null)
                {
                    terminalIterpreter.TermianlFileSystem.CreateNode (pathToPaste, true);
                    directoryToPaste = terminalIterpreter.TermianlFileSystem.FindNode (pathToPaste);
                }
                else if (terminalIterpreter.TermianlFileSystem.FindNode (pathToPaste + "/" + fileToCopy.Name) != null)
                {
                    terminalIterpreter.GneratePassiveTermialResponse ($"scp: {fileToCopy.Name} already exists in {pathToPaste}");
                }
                else
                {
                    terminalIterpreter.TermianlFileSystem.CreateNode (pathToPaste + "/" + fileToCopy.Name, false).Content = fileToCopy.Content;
                    terminalIterpreter.GneratePassiveTermialResponse ("scp: coping finish successfully!");
                }
            }
        }

        StartCoroutine (terminalIterpreter.RefreshLayout ());
    }
}
