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
    public _Dictionary<TypeOfProgram, bool> AllowedProgrames = new _Dictionary<TypeOfProgram, bool> ();
    public _Dictionary<TypeOfProgram, bool> ProgramesDownloaded = new _Dictionary<TypeOfProgram, bool> ();
    public int ActiveQuestID = -1;

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
    public string Content;
    public bool IsDirectory;
    public string ParentPath;
}

[Serializable]
public class EmailData
{
    public int ID;
    public string Subject;
    public string EmailAdress;
    public string Content;
    public string Day;
    public string Time;
    public bool Read = false;
    public bool HasAttachment;
    public bool NeedSentButton;
}

[Serializable]
public class _Dictionary <K, V>
{
    public List<K> keys = new List<K> ();
    public List<V> values = new List<V> ();
}

[Serializable]
public class ComputerList
{
    public List<ComputerData> computers;
}

[Serializable]
public class ComputerData
{
    public string Username;
    public string Password = null;
    public string IP = null;
    public List<string> Users = new List<string> ();
    public bool IsPlayer = false;
    public bool IsPasswordCracted = false;
    public int LevelOfSecurity = 0;
    public bool IsMainComputer = false;
    public bool IsDataBased = false;

    public List<SerializedNode> SystemNodes = new List<SerializedNode> ();
}

[Serializable]
public class DataBaseData
{
    public List<DataSetData> DataSets;
}

[Serializable]
public class DataSetData
{
    public string Name;
    public List<List <string>> Rows = new List<List<string>> ();
    public List<bool> FieldsVisibility = new List<bool> ();
    public List<bool> RowsVisibility = new List<bool> ();
}

[Serializable]
public class TaskData
{
    public string Name;
    public string Description;
    public int Type;
    public SerializedNode File;
    public string ComputerIP;
    public int Program;
    public string A_IP;
    public string B_IP;
}

[SerializeField]
public class QuestListData
{
    public List<QuestData> Quests;
}

[Serializable]
public class QuestData
{
    public int FinalCondition;
    public List<TaskData> Tasks = new List<TaskData> ();
    public EmailData Email;

    public List<int> ActionsAfterEmailRecived;
    public List<int> ProgramesToAllow;
    public List<ComputerData> ComputersToAdd;
    public List<string> ComputersToDelate;
    public SerializedNode File;
    public string ComputerIP;
}
