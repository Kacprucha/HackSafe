using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[SerializeField]
public enum TypeOfProgram
{
    brutForse = 0,
    rainbowTables,
    dictionaryAttack,
    manInTheMiddle,
    fakeSignature
}

public static class TerminalMenager
{
    public static List<string> GenerateTermialResponseForInstalingProgram (TypeOfProgram typeOfPrpgram)
    {
        List<string> result = new List<string> ();

        switch (typeOfPrpgram)
        {
            case TypeOfProgram.brutForse:
                break;

        }

        return result;
    }

    public static IEnumerator GenerateTerminalResponseForUpdate (TerminalIterpreter terminalIterpreter)
    {
        PassiveTerminalElement terminalElement;

        terminalIterpreter.GneratePassiveTermialResponse ("Hit:1 http://archive.ubuntu.com/ubuntu focal InRelease");

        yield return new WaitForSeconds (0.7f);
        terminalIterpreter.GneratePassiveTermialResponse ("Get:2 http://archive.ubuntu.com/ubuntu focal-updates InRelease [114 kB]");

        yield return new WaitForSeconds (0.6f);
        terminalIterpreter.GneratePassiveTermialResponse ("Get:3 http://archive.ubuntu.com/ubuntu focal-backports InRelease [101 kB]");

        yield return new WaitForSeconds (0.7f);
        terminalIterpreter.GneratePassiveTermialResponse ("Get:4 http://archive.ubuntu.com/ubuntu focal-security InRelease [114 kB]");
        terminalIterpreter.GneratePassiveTermialResponse ("Fetched 329 kB in 2s (180 kB/s)");

        terminalElement = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Reading package lists...");
        yield return new WaitForSeconds (0.2f);
        terminalElement.UpdateText ("Reading package lists... Done");

        terminalIterpreter.GneratePassiveTermialResponse ("Building dependency tree");
        terminalElement = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Reading state information...");
        yield return new WaitForSeconds (0.2f);
        terminalElement.UpdateText ("Reading state information... Done");

        terminalIterpreter.GneratePassiveTermialResponse ("All packages are up to date.");

        terminalIterpreter.PlayerInputHandler.ChangeColourOfText (false);
        terminalIterpreter.PlayerInputHandler.ChangeIteractibilityOfInputField (true);
        terminalIterpreter.PlayerInputHandler.ActiveInputField ();

    }

    public static IEnumerator GenerateTermianlResponseForInstall (TerminalIterpreter terminalIterpreter, List<TypeOfProgram> typeOfPrpgrams, bool beforeAproveFaze)
    {
        PassiveTerminalElement terminalElement;

        if (beforeAproveFaze)
        {
            terminalElement = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Reading package lists...");
            yield return new WaitForSeconds (0.2f * typeOfPrpgrams.Count);
            terminalElement.UpdateText ("Reading package lists... Done");

            terminalIterpreter.GneratePassiveTermialResponse ("Building dependency tree");
            terminalElement = terminalIterpreter.GneratePassiveTermialResponseWithPossibleUpdate ("Reading state information...");
            yield return new WaitForSeconds (0.2f);
            terminalElement.UpdateText ("Reading state information... Done");

            terminalIterpreter.GneratePassiveTermialResponse ("The following NEW packages will be installed:");

            string packages = "\t";
            foreach (TypeOfProgram typeOfPrpgram in typeOfPrpgrams)
            {
                packages += typeOfPrpgram.ToString ();

                if (typeOfPrpgram != typeOfPrpgrams[typeOfPrpgrams.Count - 1])
                {
                    packages += ", ";
                }
            }
            terminalIterpreter.GneratePassiveTermialResponse (packages);

            terminalIterpreter.GneratePassiveTermialResponse ("0 upgraded, " + typeOfPrpgrams.Count.ToString () + " newly installed, 0 to remove and 0 not upgraded.");
            terminalIterpreter.GneratePassiveTermialResponse ("Need to get " + GetSizeOfArchive (typeOfPrpgrams) + " kB of archives.");
            terminalIterpreter.GneratePassiveTermialResponse ("After this operation, " + GetSizeOfProgramm (typeOfPrpgrams) + " MB of additional disk space will be used.");
            terminalIterpreter.GneratePassiveTermialResponse ("Do you want to continue? [Y/n]");

            terminalIterpreter.PlayerInputHandler.ChangeColourOfText (false);
            terminalIterpreter.PlayerInputHandler.ChangeIteractibilityOfInputField (true);
            terminalIterpreter.PlayerInputHandler.ActiveInputField ();
        }
        else
        {
            for (int i = 0; i < typeOfPrpgrams.Count; i++)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("Get: " + (i + 1).ToString () + " " + GetURLForProgramm (typeOfPrpgrams[i]) + " [" + GetSizeOfArchive(typeOfPrpgrams[i]) + "kB]");
                yield return new WaitForSeconds (GetSizeOfArchive (typeOfPrpgrams[i]) / 180);
            }

            terminalIterpreter.GneratePassiveTermialResponse ("Fetched " + GetSizeOfArchive (typeOfPrpgrams) + " kB in " + (GetSizeOfArchive (typeOfPrpgrams) / 180).ToString() + "s (180 kB/s)");

            for (int i = 0; i < typeOfPrpgrams.Count; i++)
            {
                float test = GetSizeOfArchive (typeOfPrpgrams[i]);
                terminalIterpreter.GneratePassiveTermialResponse ("Selecting previously unselected package " + typeOfPrpgrams[i].ToString ());
                terminalIterpreter.GneratePassiveTermialResponse ("Preparing to unpack " + GetPackageForProgramm (typeOfPrpgrams[i]));
                yield return new WaitForSeconds (GetSizeOfArchive (typeOfPrpgrams[i]));

                terminalIterpreter.GneratePassiveTermialResponse ("Unpacking " + typeOfPrpgrams[i].ToString () + " ...");
                yield return new WaitForSeconds (GetSizeOfArchive (typeOfPrpgrams[i]) * 5);
            }

            for (int i = 0; i < typeOfPrpgrams.Count; i++)
            {
                terminalIterpreter.GneratePassiveTermialResponse ("Setting up " + typeOfPrpgrams[i].ToString ());
            }

            terminalIterpreter.GneratePassiveTermialResponse ("Processing triggers for man-db (2.9.1-1) ...");
            yield return new WaitForSeconds (1f);
            terminalIterpreter.GneratePassiveTermialResponse ("Processing triggers for libc-bin (2.31-0ubuntu9.9) ...");

            terminalIterpreter.PlayerInputHandler.ChangeIteractibilityOfInputField (true);
            terminalIterpreter.PlayerInputHandler.ActiveInputField ();

            terminalIterpreter.StopAptProgram ();
        }
    }

    public static bool CheckIfPlayerCanDownloadProgram (TypeOfProgram typeOfPrpgram)
    {
        bool result = false;

        GameState gameState = GameState.instance;

        if (gameState.GetPlayerInfo ().ProgramesAllowedToDownload.ContainsKey (typeOfPrpgram))
        {
            gameState.GetPlayerInfo ().ProgramesAllowedToDownload.TryGetValue (typeOfPrpgram, out result);
        }

        return result;
    }

    public static bool CheckIfPlayerDownloadedProgram (TypeOfProgram typeOfPrpgram)
    {
        bool result = false;

        GameState gameState = GameState.instance;

        if (gameState.GetPlayerInfo ().ProgramesDownloaded.ContainsKey (typeOfPrpgram))
        {
            gameState.GetPlayerInfo ().ProgramesDownloaded.TryGetValue (typeOfPrpgram, out result);
        }

        return result;
    }

    public static bool CheckIfResponseIsYes (string response)
    {
        if (response.ToLower () == "y" || response.ToLower () == "yes" || response == "Yes")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static float GetSizeOfArchive (TypeOfProgram typeOfPrpgram)
    {
        float result = 0f;

        switch (typeOfPrpgram)
        {
            case TypeOfProgram.brutForse:
                result += 0.58f;
                break;

            case TypeOfProgram.rainbowTables:
                result += 3.103f;
                break;

            case TypeOfProgram.dictionaryAttack:
                result += 1.971f;
                break;

            case TypeOfProgram.manInTheMiddle:
                result += 3.575f;
                break;

            case TypeOfProgram.fakeSignature:
                result += 2.134f;
                break;
        }

        return result;
    }

    static float GetSizeOfArchive (List<TypeOfProgram> typeOfPrpgrams)
    {
        float result = 0;

        foreach (TypeOfProgram typeOfPrpgram in typeOfPrpgrams)
        {
            switch (typeOfPrpgram)
            {
                case TypeOfProgram.brutForse:
                    result += 0.58f;
                    break;

                case TypeOfProgram.rainbowTables:
                    result += 3.103f;
                    break;

                case TypeOfProgram.dictionaryAttack:
                    result += 1.971f;
                    break;

                case TypeOfProgram.manInTheMiddle:
                    result += 3.575f;
                    break;

                case TypeOfProgram.fakeSignature:
                    result += 2.134f;
                    break;
            }
        }

        return result;
    }

    static float GetSizeOfProgramm (TypeOfProgram typeOfPrpgram)
    {
        float result = 0;

        switch (typeOfPrpgram)
        {
            case TypeOfProgram.brutForse:
                result += 2.135f;
                break;

            case TypeOfProgram.rainbowTables:
                result += 2.413f;
                break;

            case TypeOfProgram.dictionaryAttack:
                result += 3.027f;
                break;

            case TypeOfProgram.manInTheMiddle:
                result += 4.2f;
                break;

            case TypeOfProgram.fakeSignature:
                result += 2.201f;
                break;
        }

        return result;
    }

    static float GetSizeOfProgramm (List<TypeOfProgram> typeOfPrpgrams)
    {
        float result = 0;

        foreach (TypeOfProgram typeOfPrpgram in typeOfPrpgrams)
        {
            switch (typeOfPrpgram)
            {
                case TypeOfProgram.brutForse:
                    result += 2.135f;
                    break;

                case TypeOfProgram.rainbowTables:
                    result += 2.413f;
                    break;

                case TypeOfProgram.dictionaryAttack:
                    result += 3.027f;
                    break;

                case TypeOfProgram.manInTheMiddle:
                    result += 4.2f;
                    break;

                case TypeOfProgram.fakeSignature:
                    result += 2.201f;
                    break;
            }
        }

        return result;
    }

    static string GetURLForProgramm (TypeOfProgram typeOfPrpgram)
    {
        string result = "";

        switch (typeOfPrpgram)
        {
            case TypeOfProgram.brutForse:
                result = "http://archive.ubuntu.com/ubuntu focal/universe amd64 bruteForce 3.0-1";
                break;

            case TypeOfProgram.rainbowTables:
                result = "http://archive.ubuntu.com/ubuntu focal/universe amd64 rainbowTables 3.0-1";
                break;

            case TypeOfProgram.dictionaryAttack:
                result = "http://archive.ubuntu.com/ubuntu focal/universe amd64 dictionaryAttack 3.0-1";
                break;

            case TypeOfProgram.manInTheMiddle:
                result = "http://archive.ubuntu.com/ubuntu focal/universe amd64 manInTheMiddleAttack 3.0-1";
                break;

            case TypeOfProgram.fakeSignature:
                result = "http://archive.ubuntu.com/ubuntu focal/universe amd64 fakeSignature 3.0-1";
                break;
        }

        return result;
    }

    static string GetPackageForProgramm (TypeOfProgram typeOfPrpgram)
    {
        string result = "";

        switch (typeOfPrpgram)
        {
            case TypeOfProgram.brutForse:
                result = "../3-bruteForce_3.0-1_amd64.deb";
                break;

            case TypeOfProgram.rainbowTables:
                result = ".../4-rainbowTables_2.5-1_amd64.deb";
                break;

            case TypeOfProgram.dictionaryAttack:
                result = ".../5-dictionaryAttack_1.9-2_amd64.deb";
                break;

            case TypeOfProgram.manInTheMiddle:
                result = ".../6-ManInTheMiddleAttack_4.2-4_amd64.deb";
                break;

            case TypeOfProgram.fakeSignature:
                result = ".../7-fakeSignature_2.1-1_amd64.deb";
                break;
        }

        return result;
    }
}
