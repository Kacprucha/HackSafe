using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EmailMenager
{
    public static Email GetEmailOfId (int id, string day, string time)
    {
        LocalizationManager localizationManager = LocalizationManager.Instance;

        string title = localizationManager.GetLocalizedValue ("EmailTitle" + id.ToString ());
        string content = localizationManager.GetLocalizedValue ("EmailContent" + id.ToString ());

        Email result = new Email ();
        //result.Inicialize (title, , day, time);

        return result;
    }
}
