using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelOfSecurity
{
    None = 0,
    Low,
    Medium,
    High
}

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
        set { isPasswordCracted = value; }
    }

    public LevelOfSecurity SecurityLevel
    {
        get { return securityLevel; }
    }

    public bool IsMainComputer
    {
        get { return isMainComputer; }
    }

    public bool IsDataBased
    {
        get { return isDataBased; }
    }

    protected string ip = "";
    protected FileSystem fileSystem;

    protected string passwrod = "";
    protected bool isPasswordCracted = false;
    public LevelOfSecurity securityLevel = LevelOfSecurity.Low;

    protected string username = "";
    protected List<string> users;

    protected bool isPlayer = false;
    protected bool isMainComputer = false;
    protected bool isDataBased = false;

    public Computer (GameData data, int computerID = -1)
    {
        LoadData (data, computerID);
    }

    public Computer (string username, int levelOfSeciurity, bool isPlayer, string password = null, string ip = null, bool isMain = false, bool isDataBased = false)
    {
        Username = username;
        users = new List<string> { Username.Replace (" ", ""), "admin" };

        this.isPlayer = isPlayer;
        this.isMainComputer = isMain;
        this.isDataBased = isDataBased;

        if (password == null)
        {
            this.passwrod = SystemHelper.GeneratePassword (12);
        }
        else 
        {
            this.passwrod = password;
        }

        this.securityLevel = (LevelOfSecurity) levelOfSeciurity;

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
            users = new List<string> { Username, "admin" };
            isPlayer = true;
            isMainComputer = false;

            fileSystem = new FileSystem ();
            fileSystem.LoadData (data);
        }
        else
        {
            ComputerData computerData = data.CompanyComputers[computerID];
            Username = computerData.Username;
            passwrod = computerData.Password;
            IP = computerData.IP;
            users = computerData.Users;
            isPlayer = computerData.IsPlayer;
            isMainComputer = computerData.IsMainComputer;
            isDataBased = computerData.IsDataBased;
            isPasswordCracted = computerData.IsPasswordCracted;
            securityLevel = (LevelOfSecurity)computerData.LevelOfSecurity;

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
            computerData.Users = users;
            computerData.IsPlayer = isPlayer;
            computerData.IsMainComputer = isMainComputer;
            computerData.IsDataBased = isDataBased;
            computerData.IsPasswordCracted = isPasswordCracted;
            computerData.LevelOfSecurity = (int)securityLevel;

            data.CompanyComputers.Add (computerData);
            fileSystem.SaveData (ref data, data.CompanyComputers.Count - 1);
        }
    }

    public void InicializeUsers (List<string> users)
    {
        this.users = users;
    }

    public bool CheckIfGivenPasswordIsCorrect (string givenPassword)
    {
        return givenPassword.Equals (passwrod);
    }

    public bool CheckIfUserExists (string username)
    {
        return users.Contains (username);
    }
}
