using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Email 
{
    public int Id { get; private set; }

    public string Subject { get; private set; }
    public string EmailAdress { get; private set; }

    public string Content { get; private set; }
    public string Day { get; private set; }
    public string Time { get; private set; }

    public bool EmailRead { get; private set; }

    public bool OpenAttachemntButtonNecesery { get; private set; }
    public bool SentButtonNecesery { get; private set; }

    public void SetEmialRead (bool value)
    {
        EmailRead = value;
    }

    public void Inicialize (string title, string email, string content, string day, string time, bool sentButton, bool atachmentButton = false, bool read = false)
    {
        this.Subject = title;
        this.EmailAdress = email;
        this.Content = content;
        this.Day = day;
        this.Time = time;

        this.EmailRead = read;

        this.OpenAttachemntButtonNecesery = atachmentButton;
        this.SentButtonNecesery = sentButton;
    }
}
