using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Computer 
{
    public string Username 
    {
        get 
        {
            return username;
        } 
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                username = value;
            }
            else
            {
                Debug.LogError ("Username can't be empty or null!!");
            }
        } 
    }

    public string Password
    {
        get
        {
            return passwrod;
        }
        set
        {
            if (!string.IsNullOrEmpty (value))
            {
                passwrod = value;
            }
            else
            {
                Debug.LogError ("Password can't be empty or null!!");
            }
        }
    }

    public string IP
    {
        get { return ip; }
        set
        {
            if (string.IsNullOrEmpty (value))
            {
                if (isPlayer)
                {
                    ip = SystemHelper.GenerateValidIp ();
                }
                else
                {
                    ip = SystemHelper.GenerateValidIpInNetwork ();
                }
            }
            else
            {
                if (SystemHelper.IpIsCorrect (value))
                {
                    ip = value;
                }
                else
                {
                    Debug.LogError ("Given ip is inncorect!!!");
                }
            }
        }
    }

    public FileSystem FileSystem 
    {
        get { return fileSystem; }
    } 

    public bool IsPasswordCracted
    {
        get { return isPasswordCracted; }
    }

    public bool IsMainComputer
    {
        get { return isMainComputer; }
    }

    protected string ip = "";
    protected FileSystem fileSystem;

    protected string passwrod = "";
    protected bool isPasswordCracted = false;

    protected string username = "";

    protected bool isPlayer = false;
    protected bool isMainComputer = false;

    public Computer (GameData data, int computerID = -1)
    {
        LoadData (data, computerID);
    }

    public Computer (string username, bool isPlayer, string password = null, string ip = null, bool isMain = false)
    {
        Username = username;
        this.isPlayer = isPlayer;
        this.isMainComputer = isMain;

        if (password == null)
        {
            this.passwrod = SystemHelper.GeneratePassword (12);
        }
        else 
        {
            this.passwrod = password;
        }

        IP = ip;
        fileSystem = new FileSystem ();

        fileSystem.CreateNode ("/home/" + Username.Replace (" ", "") + "/documents", true);
        fileSystem.CreateNode ("/home/" + Username.Replace (" ", "") + "/desktop", true);
    }

    public void LoadData (GameData data, int computerID = -1)
    {
        if (computerID == -1)
        {
            Username = data.PlayerName;
            Password = data.PlayerPasswored;
            IP = data.PlayerIP;
            isPlayer = true;

            fileSystem = new FileSystem ();
            fileSystem.LoadData (data);
        }
        else
        {
            ComputerData computerData = data.CompanyComputers[computerID];
            Username = computerData.Username;
            this.passwrod = computerData.Password;
            IP = computerData.IP;
            isPlayer = computerData.IsPlayer;
            isPasswordCracted = computerData.IsPasswordCracted;

            fileSystem = new FileSystem ();
            fileSystem.LoadData (data, computerID);
        }
    }

    public void SaveData (ref GameData data)
    {
        if (isPlayer)
        {
            data.PlayerName = Username;
            data.PlayerPasswored = Password;
            data.PlayerIP = IP;

            fileSystem.SaveData (ref data);
        }
        else
        {
            ComputerData computerData = new ComputerData ();
            computerData.Username = Username;
            computerData.Password = Password;
            computerData.IP = IP;
            computerData.IsPlayer = isPlayer;
            computerData.IsPasswordCracted = isPasswordCracted;

            data.CompanyComputers.Add (computerData);
            fileSystem.SaveData (ref data, data.CompanyComputers.Count - 1);
        }
    }

    public bool CheckIfGivenPasswordIsCorrect (string givenPassword)
    {
        return givenPassword.Equals (passwrod);
    }
}
