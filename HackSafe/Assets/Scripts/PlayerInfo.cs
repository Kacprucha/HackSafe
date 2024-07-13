using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo 
{
    public Computer PlayerComputer { get; private set; }

    public PlayerInfo (GameData data)
    {
        LoadData (data);
    }

    public PlayerInfo (string username, string password, string ip)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty (password) && !string.IsNullOrEmpty (ip))
            PlayerComputer = new Computer (username, password, ip);
        else
            PlayerComputer = new Computer (username, password);
    }

    public void LoadData (GameData data)
    {
        PlayerComputer = new Computer (data, true);
    }

    public void SaveData (ref GameData data)
    {
        PlayerComputer.SaveData (ref data, true);
    }
}
