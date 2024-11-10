using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Purchasing;

[Serializable]
public class GameState
{
    public static GameState instance { get; private set; }

    public List<Computer> ComapnysComputers { get { return comapnysComputers; } }
    public Quest ActiveQuest { get { return activeQuest; } }

    protected PlayerInfo player;
    protected List<Computer> comapnysComputers;
    protected List<Quest> quests;
    protected Quest activeQuest;

    public GameState (GameData gameData) 
    {
        LoadData (gameData);
        instance = this;
    }

    public GameState (string playerUsername, string playerPassword, string playerIP = null)
    {
        player = new PlayerInfo (playerUsername, playerPassword, playerIP);

        loadCompanyComputerData ();
        loadQuestData ();

        instance = this;
    }

    public void LoadData (GameData data)
    {
        player = new PlayerInfo (data);

        comapnysComputers = new List<Computer> ();
        for (int i = 0; i < data.CompanyComputers.Count; i++)
        {
            comapnysComputers.Add (new Computer (data, i));
        }

        loadQuestData ();
        activeQuest = GetQuestOfId (player.ActiveQuestID);
    }

    public void SaveData (ref GameData data)
    {
        player.SaveData (ref data);

        data.CompanyComputers.Clear ();
        foreach (Computer computer in comapnysComputers)
        {
            computer.SaveData (ref data);
        }
    }

    public PlayerInfo GetPlayerInfo ()
    {
        return player;
    }

    public Computer FindComputerOfIP (string ip)
    {
        foreach (Computer computer in comapnysComputers)
        {
            if (computer.IP == ip)
                return computer;
        }

        return null;
    }

    public List<string> GetNamesOfDataBases ()
    {
        List<string> result = new List<string> ();

        foreach (Computer computer in comapnysComputers)
        {
            if (computer.IsDataBased)
                result.Add (computer.Username);
        }

        return result;
    }

    public Quest GetQuestOfId (int id)
    {
        Quest result = null;

        foreach (Quest quest in quests)
        {
            if (quest.ID == id)
            {
                result = quest;
                break;
            }
        }

        return result;
    }

    public void SetActiveQuest (Quest quest)
    {
        activeQuest = quest;
        player.ActiveQuestID = quest.ID;
    }
    private void loadCompanyComputerData ()
    {
        string fullPath = Application.persistentDataPath + "/computersData.json";
        ComputerList loadedData = null;

        if (File.Exists (fullPath))
        {
            string json = File.ReadAllText (fullPath);

            loadedData = JsonUtility.FromJson<ComputerList> (json);
        }

        comapnysComputers = new List<Computer> ();

        if (loadedData != null && loadedData.computers != null && loadedData.computers.Count > 0)
        {
            foreach (ComputerData computerData in loadedData.computers)
            {
                Computer computer = new Computer (
                                                computerData.Username, 
                                                computerData.LevelOfSecurity, 
                                                computerData.IsPlayer, 
                                                computerData.Password, 
                                                computerData.IP, 
                                                computerData.IsMainComputer, 
                                                computerData.IsDataBased
                                                );

                if (computerData.SystemNodes != null && computerData.SystemNodes.Count > 0)
                    computer.FileSystem.LoadNodesFromList (computerData.SystemNodes);

                if (computerData.Users != null && computerData.Users.Count > 0)
                    computer.InicializeUsers (computerData.Users);

                comapnysComputers.Add (computer);
            }
        }
        else
        {
            comapnysComputers.Add (new Computer ("Main Server", 0, false, "", null, true));
            comapnysComputers.Add (new Computer ("Main Data Base", 0, false, "", null, false, true));
            comapnysComputers.Add (new Computer ("Dave phone", 1, false, "1592"));
            comapnysComputers.Add (new Computer ("Arasha PC", 2, false));
            comapnysComputers.Add (new Computer ("#$^)#$@#)^#!", 3, false));
        }
    }

    private void loadQuestData ()
    {
        string fullPath = Application.persistentDataPath + "/questData.json";
        QuestListData loadedData = null;

        if (File.Exists (fullPath))
        {
            string json = File.ReadAllText (fullPath);

            loadedData = JsonUtility.FromJson<QuestListData> (json);
        }

        quests = new List<Quest> ();

        if (loadedData != null && loadedData.Quests != null && loadedData.Quests.Count > 0)
        {
            foreach (QuestData questData in loadedData.Quests)
            {
                int questID = questData.Email.ID;
                TaskType finalCondition = (TaskType)questData.FinalCondition;

                List <Task> tasks = new List<Task> ();
                if (questData.Tasks != null && questData.Tasks.Count > 0)
                {
                    for (int i = 0; i < questData.Tasks.Count; i++)
                    {
                        Task task = new Task (questData.Tasks[i], i);
                        tasks.Add (task);
                    }
                }

                List<ActionsAfterRecivingEmail> actionsAfterRecivingEmail = new List<ActionsAfterRecivingEmail> ();
                foreach (int action in questData.ActionsAfterEmailRecived)
                {
                    actionsAfterRecivingEmail.Add ((ActionsAfterRecivingEmail) action);
                }

                List<TypeOfProgram> typeOfPrograms = new List<TypeOfProgram> ();
                if (questData.ProgramesToAllow != null && questData.ProgramesToAllow.Count > 0)
                {
                    foreach (int program in questData.ProgramesToAllow)
                    {
                        typeOfPrograms.Add ((TypeOfProgram)program);
                    }
                }

                List<Computer> computers = new List<Computer> ();
                if (questData.ComputersToAdd != null && questData.ComputersToAdd.Count > 0)
                {
                    foreach (ComputerData computerData in questData.ComputersToAdd)
                    {
                        Computer computer = new Computer (
                                                        computerData.Username,
                                                        computerData.LevelOfSecurity,
                                                        computerData.IsPlayer,
                                                        computerData.Password,
                                                        computerData.IP,
                                                        computerData.IsMainComputer,
                                                        computerData.IsDataBased
                                                        );

                        if (computerData.SystemNodes != null && computerData.SystemNodes.Count > 0)
                            computer.FileSystem.LoadNodesFromList (computerData.SystemNodes);

                        if (computerData.Users != null && computerData.Users.Count > 0)
                            computer.InicializeUsers (computerData.Users);

                        computers.Add (computer);
                    }
                }

                List<string> computersToDelate = new List<string> ();
                if (questData.ComputersToDelate != null && questData.ComputersToDelate.Count > 0)
                    computersToDelate.AddRange (questData.ComputersToDelate);

                TreeNode file = null;
                if (questData.File != null)
                    file = new TreeNode (questData.File.Name, true, questData.File.Content);

                Quest quest = new Quest (
                                        questID, 
                                        finalCondition, 
                                        questData.Email, 
                                        tasks, 
                                        actionsAfterRecivingEmail,
                                        typeOfPrograms, 
                                        computers,
                                        computersToDelate,
                                        file,
                                        questData.ComputerIP
                                        );

                quests.Add (quest);
                Debug.Log ("Quest loaded: " + questID);
            }
        }
    }
}
