using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EmailMenager
{
    public static Email GetEmailFromeData (EmailData emailData)
    {
        Email result = new Email ();
        result.Inicialize (
                        emailData.ID,
                        emailData.Subject, 
                        emailData.EmailAdress, 
                        emailData.Content, 
                        emailData.Day, 
                        emailData.Time, 
                        emailData.NeedSentButton, 
                        emailData.HasAttachment, 
                        emailData.Read
                        );

        return result;
    }

    public static Email GetEmailOfId (int id)
    { 
        Email result = new Email ();
        result = GetEmailFromeData (GameState.instance.GetQuestOfId (id).EmailData);

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

    public static void CheckIfEmailNeedAnyAction (int id, bool emailRead)
    {
        GameState gameState = GameState.instance;
        PlayerInfo player = gameState.GetPlayerInfo ();

        switch (id)
        {
            case 0:
                if (player != null && !emailRead)
                {
                    player.ProgramesAllowedToDownload[TypeOfProgram.brutForse] = true;
                    player.ProgramesAllowedToDownload[TypeOfProgram.rainbowAttack] = true;
                    player.ProgramesAllowedToDownload[TypeOfProgram.dictionaryAttack] = true;

                    Debug.Log ("Programs anabled: bruteForce, rainbow, dicionary");
                }

                break;
        }
    }
}
