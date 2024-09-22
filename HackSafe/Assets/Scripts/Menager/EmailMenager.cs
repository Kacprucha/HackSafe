using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EmailMenager
{
    public static Email GetEmailOfId (int id, string day, string time, bool read)
    {
        LocalizationManager localizationManager = LocalizationManager.Instance;

        string title = localizationManager.GetLocalizedValue ("EmailTitle" + id.ToString ());
        string content = localizationManager.GetLocalizedValue ("EmailContent" + id.ToString ());

        Email result = new Email ();
        result.Inicialize (title, GetAdresOfId (id), content, day, time, GetSentButtonNeceseryOfId (id), GetAttachmentButtonNeceseryOfId (id), read);

        return result;
    }

    public static string GetAdresOfId (int id)
    {
        string result = "example.example@email.com";

        switch (id)
        {
            case 0:
                result = "potato.company@email.com";
                break;
        }

        return result;
    }

    public static bool GetSentButtonNeceseryOfId (int id)
    {
        bool result = false;

        switch (id)
        {
            case 0:
                result = false;
                break;
        }

        return result;
    }

    public static bool GetAttachmentButtonNeceseryOfId (int id)
    {
        bool result = false;

        switch (id)
        {
            case 0:
                result = false;
                break;
        }

        return result;
    }
}
