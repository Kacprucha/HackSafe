using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public string PlayerName;
    public string PlayerPasswored;
    public string PlayerIP;
    public List<SerializedNode> PlayerNodes = new List<SerializedNode> ();
    public List<EmailData> RecivedEmails = new List<EmailData> ();

    public GameData ()
    {
        PlayerName = "";
        PlayerPasswored = "";
        PlayerIP = "";
    }
}

[Serializable]
public class SerializedNode
{
    public string Name;
    public bool IsDirectory;
    public string ParentPath;
}

[Serializable]
public class EmailData
{
    public int ID;
    public string Day;
    public string Time;
    public bool Read;
}
