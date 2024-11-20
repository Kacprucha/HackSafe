using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GenerateFileForFNSView : MonoBehaviour
{
    [SerializeField] InputField inputForDocument;

    [SerializeField] Button generateNumberButton;
    [SerializeField] Text numberLabel;

    [SerializeField] InputField inputForKeys;

    [SerializeField] Button generateFileToSigne;
    [SerializeField] Text systemMEssageLabel;

    protected int number;

    public delegate void SendFileHandler (TreeNode file);
    public event SendFileHandler OnSentFile;

    // Start is called before the first frame update
    void Start ()
    {
        if (generateNumberButton != null)
        {
            generateNumberButton.onClick.AddListener (() => generateNumberClicked ());
        }

        if (generateFileToSigne != null)
        {
            generateFileToSigne.onClick.AddListener (() => generateFileClicked ());
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void OnDisable ()
    {
        numberLabel.text = "";
        systemMEssageLabel.text = "";
        numberLabel.text = "-";
        number = 0;
    }

    protected void generateNumberClicked ()
    {
        number = Random.Range (1, 1000);
        numberLabel.text = number.ToString ();
    }

    protected void generateFileClicked ()
    {
        FileSystem fileSystem = GameState.instance.GetPlayerInfo ().PlayerComputer.FileSystem;

        string pathToFile = inputForDocument.text;
        string pathToKeys = inputForKeys.text;

        bool filesExists = fileSystem.FindNode (pathToFile) != null && fileSystem.FindNode (pathToKeys) != null;

        if (pathToFile.NullIfEmpty () == null || pathToKeys.NullIfEmpty () == null)
        {
            systemMEssageLabel.text = "You have to enter path for both files!";
        }
        else if (!filesExists)
        {
            systemMEssageLabel.text = "One or both files don't egists on given path!";
        }
        else if (fileSystem.FindNode (pathToKeys).IsKeyFile)
        {
            systemMEssageLabel.text = "File pass as a key don't containe keys!";
        }
        else if (OnSentFile != null)
        {
            OnSentFile (fileSystem.FindNode (pathToFile));
        }
    }
}
