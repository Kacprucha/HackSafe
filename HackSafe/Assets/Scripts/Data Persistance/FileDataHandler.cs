using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileDataHandler
{
    static string dataDirPath = Application.persistentDataPath;
    static string dataFileName = "save.game";

    public GameData Load ()
    {
        string fullPath = Path.Combine (dataDirPath, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream (fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader (stream))
                    {
                        dataToLoad = reader.ReadToEnd ();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData> (dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError ("Error occured when trying to load data to file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save (GameData data)
    {
        string fullPath = Path.Combine (dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory (Path.GetDirectoryName (fullPath));

            string jsonData = JsonUtility.ToJson (data, true);

            using (FileStream stream = new FileStream (fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write (jsonData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError ("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
