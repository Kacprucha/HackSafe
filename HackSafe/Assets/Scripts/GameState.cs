using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public static GameState instance { get; private set; }

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

        instance = this;
    }

    public void LoadData (GameData data)
    {
        player = new PlayerInfo (data);
    }

    public void SaveData (ref GameData data)
    {
        player.SaveData (ref data);
    }

    public PlayerInfo GetPlayerInfo ()
    {
        return player;
    }
}
