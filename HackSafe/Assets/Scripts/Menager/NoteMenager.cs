using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteMenager : MonoBehaviour
{
    [SerializeField] Text systemNote;
    [SerializeField] Text linuxNote;

    [SerializeField] int ammountOfLinuxNotes;

    // Start is called before the first frame update
    void Start()
    {
        populateLinuxNotes ();
    }

    // Update is called once per frame
    void Update()
    {
        populateNote ();   
    }

    protected void populateNote ()
    {
        LocalizationManager localizationManager = LocalizationManager.Instance;

        PlayerInfo player = GameState.instance.GetPlayerInfo ();
        systemNote.text = "";

        foreach (TypeOfProgram programe in Enum.GetValues (typeof (TypeOfProgram)))
        {
            if (player.ProgramesDownloaded[programe])
            {
                systemNote.text += localizationManager.GetLocalizedValue (programe.ToString ()) + "\n";
            }
        }
    }

    protected void populateLinuxNotes ()
    {
        LocalizationManager localizationManager = LocalizationManager.Instance;

        PlayerInfo player = GameState.instance.GetPlayerInfo ();
        linuxNote.text = "";
        string baseName = "command";

        for (int i = 0; i < ammountOfLinuxNotes; i++)
        {
            string commandName = baseName + i.ToString ();
            systemNote.text += localizationManager.GetLocalizedValue (commandName) + "\n";
        }
    }
}
