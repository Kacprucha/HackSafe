using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    public  static string[] Languages = { "English", "Polski" };
    static string pathForLolalizationFile = Path.Combine (Application.dataPath, "Resources/LocalizationData.csv");

    public string CurrentLanguage = "English";

    private Dictionary<string, string> localizedText;

    private void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad (gameObject);
        }
        else
        {
            Destroy (gameObject);
        }

        LoadLocalizedText ();
    }

    public void LoadLocalizedText ()
    {
        localizedText = new Dictionary<string, string> ();

        if (File.Exists (pathForLolalizationFile))
        {
            string[] data = File.ReadAllLines (pathForLolalizationFile);

            int languageIndex = getIndexOfLanguage (CurrentLanguage);

            for (int i = 1; i < data.Length; i++)
            {
                string[] lineData = data[i].Split (';');

                string key = lineData[0];
                localizedText.Add (key, lineData[languageIndex]);
            }

            Debug.Log ("Localization data loaded.");
        }
        else
        {
            Debug.LogError ("Cannot find file: " + pathForLolalizationFile);
        }
    }

    public string GetLocalizedValue (string key)
    {
        if (localizedText.ContainsKey (key))
        {
            if (getIndexOfLanguage (CurrentLanguage) > 0)
            {
                return localizedText[key];
            }
            else
            {
                Debug.LogWarning ("Language not found: " + CurrentLanguage);
                return key;
            }
        }
        else
        {
            Debug.LogWarning ("Key not found: " + key);
            return key;
        }
    }

    public void SetLanguage (string language)
    {
        CurrentLanguage = language;
        LoadLocalizedText ();
    }

    private int getIndexOfLanguage (string language)
    {
        int result = -1;

        switch (language)
        {
            case "English":
                result = 1;
                break;

            case "Polski":
                result = 2;
                break;
        }

        return result;
    }
}