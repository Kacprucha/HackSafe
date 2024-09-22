using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo 
{
    public Computer PlayerComputer { get; private set; }
    public List<Email> RecivedEmails { get; private set; }

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

        RecivedEmails = new List<Email> ();
    }

    public void LoadData (GameData data)
    {
        PlayerComputer = new Computer (data, true);

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
    }

    public void SaveData (ref GameData data)
    {
        PlayerComputer.SaveData (ref data, true);

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
    }
}
