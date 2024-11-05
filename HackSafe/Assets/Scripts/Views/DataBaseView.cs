using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBaseView : MonoBehaviour
{
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject boldCellPrefab;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject tableNamePrefab;

    [SerializeField] GameObject nameTabContainer;
    [SerializeField] GameObject scrollViewContent;

    [SerializeField] InputField inputField;
    [SerializeField] Button subbmitButton;
    [SerializeField] Text errorMessageLabel;

    static Color normalColor = new Color (1, 1, 1, 1);
    static Color selectedColor = new Color (0.6603774f, 0.6603774f, 0.6603774f, 1);

    protected string dataBaseIp;
    protected DataBaseLogic dataBaseLogic;
    protected int currentTabID = 0;
    protected List<GameObject> rows = new List<GameObject> ();
    protected List<Graphic> tabBackgrounds = new List<Graphic> ();

    // Start is called before the first frame update
    void Start()
    {
        if (dataBaseLogic == null)
        {
            dataBaseLogic = new DataBaseLogic ();
            dataBaseLogic.OnSQLError += handleSQLError;
        }

        if (subbmitButton != null)
            subbmitButton.onClick.AddListener (() => onSubmitButtonClicked ());
    }

    // Update is called once per frame
    void Update()
    {
        if (tabBackgrounds.Count > 0)
        {
            for (int i = 0; i < tabBackgrounds.Count; i++)
            {
                if (i == currentTabID)
                    tabBackgrounds[i].color = selectedColor;
                else
                    tabBackgrounds[i].color = normalColor;
            }
        }
    }

    void OnDestroy ()
    {
        dataBaseLogic.DeleteDatabase ();
    }

    public void Show (string dataBaseIp)
    {
        this.dataBaseIp = dataBaseIp;

        gameObject.SetActive (true);

        if (dataBaseLogic == null)
        {
            dataBaseLogic = new DataBaseLogic ();
            dataBaseLogic.OnSQLError += handleSQLError;
        }

        if (dataBaseLogic.DataBaseDatas.Count == 0)
        {
            InicializeDataBases ();
        }

        InicializeView ();
    }

    public void Hide ()
    {
        gameObject.SetActive (false);
    }

    public void InicializeDataBases ()
    {
        dataBaseLogic.LoadDataBaseData ();
    }

    public void InicializeView ()
    {
        GameState gameState = GameState.instance;
        currentTabID = 0;
        rows.Clear ();

        string currentDbName = gameState.FindComputerOfIP (dataBaseIp).Username;
        DataBaseData dataBaseData = dataBaseLogic.DataBaseDatas[currentDbName];

        if (dataBaseData.DataSets.Count > 1)
        {
            for (int i = 0; i < dataBaseData.DataSets.Count; i++)
            {
                int index = i;
                GameObject newTab = Instantiate (tableNamePrefab, nameTabContainer.transform);
                newTab.GetComponent <Label> ().UpdateLabel (dataBaseData.DataSets[i].Name);
                newTab.GetComponent<Button> ().onClick.AddListener (() => onTabButtonClicked (index));
                tabBackgrounds.Add (newTab.GetComponent<Button> ().targetGraphic);
            }
        }

        if (dataBaseData.DataSets.Count > 0)
        {
            DataSetData dataSet = dataBaseData.DataSets[0];
            GameObject nameRow = Instantiate (rowPrefab, scrollViewContent.transform);
            rows.Add (nameRow);

            for (int i = 0; i < dataSet.Rows[0].Count; i++)
            {
                if (dataSet.FieldsVisibility[i])
                {
                    GameObject boldCell = Instantiate (boldCellPrefab, nameRow.transform);
                    boldCell.GetComponent<Label> ().UpdateLabel (dataSet.Rows[0][i]);
                }
            }

            for (int i = 1; i < dataSet.Rows.Count; i++)
            {
                if (dataSet.RowsVisibility[i - 1])
                {
                    GameObject vairablesRow = Instantiate (rowPrefab, scrollViewContent.transform);
                    rows.Add (vairablesRow);

                    for (int j = 0; j < dataSet.Rows[0].Count; j++)
                    {
                        if (dataSet.FieldsVisibility[j])
                        {
                            GameObject cell = Instantiate (cellPrefab, vairablesRow.transform);

                            if (j != 0)
                                cell.GetComponent<Label> ().UpdateLabel (dataSet.Rows[i][j].Substring (1, dataSet.Rows[i][j].Length - 2));
                            else
                                cell.GetComponent<Label> ().UpdateLabel (dataSet.Rows[i][j]);
                        }
                    }
                }
            }
        }
    }
    protected void handleInputOnEnter (string userInput)
    {
        GameState gameState = GameState.instance;

        if ((Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) && dataBaseLogic != null)
        {
            List<List<string>> quoteResult = dataBaseLogic.ExecuteQuery (gameState.FindComputerOfIP (dataBaseIp).Username, inputField.text);

            if (quoteResult.Count > 0)
            {
                updateDataBaseView (quoteResult);
                currentTabID = -1;
            }

            inputField.text = string.Empty;
            inputField.ActivateInputField ();
        }
    }

    protected void handleSQLError (string error)
    {
        errorMessageLabel.text = error;
    }

    protected void onSubmitButtonClicked ()
    {
        GameState gameState = GameState.instance;
        errorMessageLabel.text = string.Empty;

        List<List<string>> quoteResult = dataBaseLogic.ExecuteQuery (gameState.FindComputerOfIP (dataBaseIp).Username, inputField.text);

        if (quoteResult.Count > 0)
        {
            updateDataBaseView (quoteResult);
            currentTabID = -1;
        }

        inputField.text = string.Empty;
        inputField.ActivateInputField ();
    }

    protected void onTabButtonClicked (int tabID)
    {
        if (currentTabID != tabID)
        {
            updateDataBaseView (tabID);
            currentTabID = tabID;
        }
    }

    protected void updateDataBaseView (int tableID)
    {
        GameState gameState = GameState.instance;

        foreach (GameObject row in rows)
        {
            Destroy (row);
        }
        rows.Clear ();

        string currentDbName = gameState.FindComputerOfIP (dataBaseIp).Username;
        DataBaseData dataBaseData = dataBaseLogic.DataBaseDatas[currentDbName];

        if (dataBaseData.DataSets.Count > tableID)
        {
            DataSetData dataSet = dataBaseData.DataSets[tableID];
            GameObject nameRow = Instantiate (rowPrefab, scrollViewContent.transform);
            rows.Add (nameRow);

            for (int i = 0; i < dataSet.Rows[0].Count; i++)
            {
                if (dataSet.FieldsVisibility[i])
                {
                    GameObject boldCell = Instantiate (boldCellPrefab, nameRow.transform);
                    boldCell.GetComponent<Label> ().UpdateLabel (dataSet.Rows[0][i]);
                }
            }

            for (int i = 1; i < dataSet.Rows.Count; i++)
            {
                if (dataSet.RowsVisibility[i - 1])
                {
                    GameObject vairablesRow = Instantiate (rowPrefab, scrollViewContent.transform);
                    rows.Add (vairablesRow);

                    for (int j = 0; j < dataSet.Rows[0].Count; j++)
                    {
                        if (dataSet.FieldsVisibility[j])
                        {
                            GameObject cell = Instantiate (cellPrefab, vairablesRow.transform);

                            if (j != 0)
                                cell.GetComponent<Label> ().UpdateLabel (dataSet.Rows[i][j].Substring (1, dataSet.Rows[i][j].Length - 2));
                            else
                                cell.GetComponent<Label> ().UpdateLabel (dataSet.Rows[i][j]);
                        }
                    }
                }
            }
        }
    }

    protected void updateDataBaseView (List<List<string>> quteResults)
    {
        GameState gameState = GameState.instance;

        foreach (GameObject row in rows)
        {
            Destroy (row);
        }
        rows.Clear ();

        string currentDbName = gameState.FindComputerOfIP (dataBaseIp).Username;
        DataBaseData dataBaseData = dataBaseLogic.DataBaseDatas[currentDbName];

        if (quteResults[0].Count > 0)
        {
            GameObject nameRow = Instantiate (rowPrefab, scrollViewContent.transform);
            rows.Add (nameRow);

            for (int i = 0; i < quteResults[0].Count; i++)
            {
                GameObject boldCell = Instantiate (boldCellPrefab, nameRow.transform);
                boldCell.GetComponent<Label> ().UpdateLabel (quteResults[0][i]);
            }

            for (int i = 1; i < quteResults.Count; i++)
            {
                GameObject vairablesRow = Instantiate (rowPrefab, scrollViewContent.transform);
                rows.Add (vairablesRow);

                for (int j = 0; j < quteResults[i].Count; j++)
                {
                    GameObject cell = Instantiate (cellPrefab, vairablesRow.transform);
                    cell.GetComponent<Label> ().UpdateLabel (quteResults[i][j]);

                }
            }
        }
    }
}
