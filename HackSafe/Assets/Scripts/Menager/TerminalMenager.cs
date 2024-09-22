using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfPrpgram
{
    butForse,
    rainbowTables,
    dictionaryAttack
}

public static class TerminalMenager
{
    public static List<string> GenerateTermialResponseForInstalingProgram (TypeOfPrpgram typeOfPrpgram)
    {
        List<string> result = new List<string> ();

        switch (typeOfPrpgram)
        {
            case TypeOfPrpgram.butForse:
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

    }
}
