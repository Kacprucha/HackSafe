using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameState
{
    public static GameState instance { get; private set; }

    public List<Computer> ComapnysComputers { get { return comapnysComputers; } }

    protected PlayerInfo player;

    protected List<Computer> comapnysComputers;

    public GameState (GameData gameData) 
    {
        LoadData (gameData);
        instance = this;
    }

    public GameState (string playerUsername, string playerPassword, string playerIP = null)
    {
        player = new PlayerInfo (playerUsername, playerPassword, playerIP);

        loadCompanyComputerData ();

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
                Computer computer = new Computer (computerData.Username, computerData.LevelOfSecurity, computerData.IsPlayer, computerData.Password, computerData.IP, computerData.IsMainComputer);
                comapnysComputers.Add (computer);
            }
        }
        else
        {
            comapnysComputers.Add (new Computer ("Main Server", 0, false, ""));
            comapnysComputers.Add (new Computer ("Main Data Base", 0, false, ""));
            comapnysComputers.Add (new Computer ("Dave phone", 1, false, "15920"));
            comapnysComputers.Add (new Computer ("Arasha PC", 2, false));
            comapnysComputers.Add (new Computer ("#$^)#$@#)^#!", 3, false));
        }
    }
}

[Serializable]
public class ComputerList
{
    public List<ComputerData> computers;
}