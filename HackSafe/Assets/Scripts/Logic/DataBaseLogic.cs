using Newtonsoft.Json;
using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Mono.Data.SqliteClient;
using UnityEngine;

public class DataBaseLogic
{
    public Dictionary<string, DataBaseData> DataBaseDatas { get { return dataBaseDatas; } }

    private Dictionary <string, string> dbPaths = new Dictionary<string, string> ();
    private Dictionary<string, DataBaseData> dataBaseDatas = new Dictionary<string, DataBaseData> ();

    public void LoadDataBaseData ()
    {
        GameState gameState = GameState.instance;

        List<string> dbNames = gameState.GetNamesOfDataBases ();

        foreach (string dbName in dbNames)
        {
            string fullPath = $"{Application.persistentDataPath}/{dbName.Replace (" ", "")}Data.json";
            DataBaseData loadedData = null;

            if (File.Exists (fullPath))
            {
                string json = File.ReadAllText (fullPath);

                loadedData = JsonConvert.DeserializeObject<DataBaseData> (json);
            }

            if (loadedData != null && loadedData.DataSets != null && loadedData.DataSets.Count > 0)
            {
                AddDataBase (dbName, loadedData);
                dataBaseDatas.Add (dbName, loadedData);
            }
        }
    }

    public void AddDataBase (string dbName, DataBaseData dbData)
    {
        dbPaths.Add (dbName, $"URI=file:{Application.persistentDataPath}/{dbName.Replace (" ", "")}.db");
        crateDataBase (dbName, dbData);
    }

    public void DeleteDatabase ()
    {
        foreach (string path in dbPaths.Values)
        {
            string realPath = path.Substring (9, path.Length - 9);
            if (File.Exists (realPath))
            {
                File.Delete (realPath);
                Debug.Log ("Database file deleted successfully.");
            }
            else
            {
                Debug.Log ("No database file found to delete.");
            }
        }
    }

    public List<List<string>> ExecuteQuery (string dbName, string sqlQuery)
    {
        List<List<string>> result = new List<List<string>> ();

        using (var connection = new SqliteConnection (dbPaths[dbName]))
        {
            connection.Open ();
            using (var command = connection.CreateCommand ())
            {
                command.CommandText = sqlQuery;
                try
                {
                    using (IDataReader reader = command.ExecuteReader ())
                    {
                        int fieldCount = reader.FieldCount;
                        List<string> columnsNames = new List<string> ();

                        for (int i = 0; i < fieldCount; i++)
                        {
                            columnsNames.Add (reader.GetName (i));
                        }

                        result.Add (columnsNames);

                        Debug.Log ("Columns: " + string.Join (", ", result[0]));

                        int rowIndex = 1;
                        while (reader.Read ())
                        {
                            List<string> row = new List<string> ();
                            for (int i = 0; i < fieldCount; i++)
                            {
                                string columnName = result[0][i];
                                string columnValue = reader[columnName].ToString ();
                                row.Add (columnValue);
                            }

                            result.Add (row);

                            Debug.Log ("Row " + rowIndex.ToString() + " : " + string.Join (", ", result[rowIndex]));

                            rowIndex++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError ("SQL Error: " + e.Message);
                }
            }
        }

        return result;
    }

    private void crateDataBase (string dbName, DataBaseData dbData)
    {
        using (var connection = new SqliteConnection (dbPaths[dbName]))
        {
            connection.Open ();

            for (int i = 0; i < dbData.DataSets.Count; i++)
            {
                DataSetData currentDataSet = dbData.DataSets[i];
                using (var command = connection.CreateCommand ())
                {
                    string tableColumns = "";
                    tableColumns += $"{currentDataSet.Rows[0][0]} INTEGER PRIMARY KEY,";

                    for (int j = 1; j < currentDataSet.Rows[0].Count; j++)
                    {
                        tableColumns += $" {currentDataSet.Rows[0][j]} TEXT";
                        if (j < currentDataSet.Rows[0].Count - 1)
                        {
                            tableColumns += ", ";
                        }
                    }

                    command.CommandText = $"CREATE TABLE IF NOT EXISTS {currentDataSet.Name} ({tableColumns})"; ;
                    command.ExecuteNonQuery ();

                    for (int j = 1; j < currentDataSet.Rows.Count; j++)
                    {
                        List<string> values = currentDataSet.Rows[j].GetRange (1, currentDataSet.Rows[j].Count - 1);
                        List<string> keys = currentDataSet.Rows[0].GetRange (1, currentDataSet.Rows[0].Count - 1);
                        command.CommandText = $"INSERT INTO {currentDataSet.Name} ({string.Join (", ", keys.ToArray ())}) VALUES ({string.Join (", ", values.ToArray ())})";
                        command.ExecuteNonQuery ();
                    }
                }
            }
        }
    }
}
