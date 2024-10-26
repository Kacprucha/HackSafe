using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInfo 
{
    public Computer PlayerComputer { get; private set; }
    public List<Email> RecivedEmails { get; private set; }
    public Dictionary <TypeOfProgram, bool> ProgramesAllowedToDownload { get; private set; }
    public Dictionary<TypeOfProgram, bool> ProgramesDownloaded { get; private set; }

    public PlayerInfo (GameData data)
    {
        LoadData (data);
    }

    public PlayerInfo (string username, string password, string ip)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty (password) && !string.IsNullOrEmpty (ip))
            PlayerComputer = new Computer (username, 0, true, password, ip);
        else
            PlayerComputer = new Computer (username, 0, true ,password);

        RecivedEmails = new List<Email> ();
        inicializeProgramesAllowedToDownload ();
    }

    public void LoadData (GameData data)
    {
        PlayerComputer = new Computer (data);

        if (data.RecivedEmails != null && data.RecivedEmails.Count > 0)
        {
            RecivedEmails = new List<Email> ();

            foreach (EmailData email in data.RecivedEmails)
            {
                RecivedEmails.Add (EmailMenager.GetEmailOfId (email.ID, email.Day, email.Time, email.Read));
            }
        }
        else
        {
            RecivedEmails = new List<Email> ();
        }

        if ((data.AllowedProgrames != null && data.AllowedProgrames.keys.Count > 0) && (data.ProgramesDownloaded != null && data.ProgramesDownloaded.keys.Count >= 0))
        {
            ProgramesAllowedToDownload = data.AllowedProgrames.keys.Zip (data.AllowedProgrames.values, (k, v) => new { k, v }).ToDictionary (x => x.k, x => x.v);
            ProgramesDownloaded = data.ProgramesDownloaded.keys.Zip (data.ProgramesDownloaded.values, (k, v) => new { k, v }).ToDictionary (x => x.k, x => x.v);
        }
        else
        {
            inicializeProgramesAllowedToDownload ();
        }
    }

    public void SaveData (ref GameData data)
    {
        PlayerComputer.SaveData (ref data);

        if (RecivedEmails != null && RecivedEmails.Count > 0)
        {
            data.RecivedEmails.Clear ();

            foreach (Email email in RecivedEmails)
            {
                EmailData emailData = new EmailData ();
                emailData.ID = email.Id;
                emailData.Day = email.Day;
                emailData.Time = email.Time;
                emailData.Read = email.EmailRead;

                data.RecivedEmails.Add (emailData);
            }
        }

        if (ProgramesAllowedToDownload != null)
        {
            data.AllowedProgrames.keys = ProgramesAllowedToDownload.Keys.ToList ();
            data.AllowedProgrames.values = ProgramesAllowedToDownload.Values.ToList ();
        }

        if (ProgramesDownloaded != null)
        {
            data.ProgramesDownloaded.keys = ProgramesDownloaded.Keys.ToList ();
            data.ProgramesDownloaded.values = ProgramesDownloaded.Values.ToList ();
        }
    }

    protected void inicializeProgramesAllowedToDownload ()
    {
        ProgramesAllowedToDownload = new Dictionary<TypeOfProgram, bool> ();
        ProgramesDownloaded = new Dictionary<TypeOfProgram, bool> ();

        foreach (TypeOfProgram programe in Enum.GetValues (typeof (TypeOfProgram)))
        {
            ProgramesAllowedToDownload.Add (programe, false);
            ProgramesDownloaded.Add (programe, false);
        }
    }
}
