using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer 
{
    public string Username { get; private set; }

    protected string ip = "";
    protected FileSystem fileSystem;

    protected string passwrod = "";
    protected bool ifPasswordCracted = false;

    public string IP
    {
        get { return ip; }
        set 
        {
            if (string.IsNullOrEmpty(value))
            {
                ip = SystemHelper.GenerateValidIp ();
            }
            else
            {
                if (SystemHelper.IpIsCorrect(value))
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
