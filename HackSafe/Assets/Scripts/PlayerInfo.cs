using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo 
{
    public Computer PlayerComputer { get; private set; }

    public PlayerInfo (string username, string password, string ip)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty (password) && !string.IsNullOrEmpty (ip))
            PlayerComputer = new Computer (username, password, ip);
        else
            PlayerComputer = new Computer (username, password);
    }
}
