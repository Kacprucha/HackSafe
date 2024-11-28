using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistanceMenager : MonoBehaviour
{
    public static DataPersistanceMenager Instance { get; private set; }

    private GameData gameData;
    private List<IDataPersistance> dataPersistancesObjects;
    private FileDataHandler dataHandler;

    public DataPersistanceMenager ()
    {
        if (Instance != null)
        {
            Debug.LogError ("More then one Data Persistance Menager is in the scene!!!");
        }

        Instance = this;
    }

    private void Start ()
    {
        dataHandler = new FileDataHandler ();
        dataPersistancesObjects = findAllDataPersistanceObjects ();
        LoadGame ();
    }

    private void OnApplicationQuit ()
    {
        SaveGame ();
    }

    public void NewGame()
    {
        gameData = new GameData ();
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load ();

        if (gameData == null)
        {
            Debug.Log ("Non saved data find. Inicializing ned game data.");
            NewGame ();
        }

        foreach (IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.LoadData (gameData);
        }
    }

    public void SaveGame ()
    {
        foreach (IDataPersistance dataPersistanceObj in dataPersistancesObjects)
        {
            dataPersistanceObj.SaveData (ref gameData);
        }

        dataHandler.Save (gameData);
    }

    private List<IDataPersistance> findAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour> ().OfType<IDataPersistance>();

        return new List<IDataPersistance> (dataPersistanceObjects);
    }
}
