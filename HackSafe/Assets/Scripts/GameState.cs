using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public static GameState instance { get; private set; }

    protected PlayerInfo player;

    protected List<Computer> comapnysComputers;

    public GameState (string playerUsername, string playerPassword, string playerIP = null)
    {
        player = new PlayerInfo (playerUsername, playerPassword, playerIP);

        instance = this;
    }

    public PlayerInfo GetPlayerInfo ()
    {
        return player;
    }
}
