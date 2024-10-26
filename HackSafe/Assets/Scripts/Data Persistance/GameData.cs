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
    public _Dictionary<TypeOfPrpgram, bool> AllowedProgrames = new _Dictionary<TypeOfPrpgram, bool> ();
    public _Dictionary<TypeOfPrpgram, bool> ProgramesDownloaded = new _Dictionary<TypeOfPrpgram, bool> ();

    public List<ComputerData> CompanyComputers = new List<ComputerData> ();

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

[Serializable]
public class _Dictionary <K, V>
{
    public List<K> keys = new List<K> ();
    public List<V> values = new List<V> ();
}

[Serializable]
public class ComputerData
{
    public string Username;
    public string Password = null;
    public string IP = null;
    public bool IsPlayer = false;
    public bool IsPasswordCracted = false;
    public int LevelOfSecurity = 0;
    public bool IsMainComputer = false;

    public List<SerializedNode> SystemNodes = new List<SerializedNode> ();
}
