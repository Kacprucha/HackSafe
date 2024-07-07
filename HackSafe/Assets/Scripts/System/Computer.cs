using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                ip = SystemHelper.GenerateValidIp ();
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

    protected string ip = "";
    protected FileSystem fileSystem;

    protected string passwrod = "";
    protected bool ifPasswordCracted = false;

    protected string username = "";

    public Computer (string username, string password = null, string ip = null )
    {
        Username = username;

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

        fileSystem.CreateNode ("/home/" + Username + "/documents", true);
        fileSystem.CreateNode ("/home/" + Username + "/desktop", true);
    }

    public bool CheckIfGivenPasswordIsCorrect (string givenPassword)
    {
        return givenPassword.Equals (passwrod);
    }
}
