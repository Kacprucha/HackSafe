using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SavingFileForFNSView : MonoBehaviour
{
    [SerializeField] InputField inputForPathWhereSave;

    [SerializeField] Button saveButton;
    [SerializeField] Text systemMessageLabel;

    protected TreeNode baseFile;

    public delegate void FileSaveHandler ();
    public event FileSaveHandler OnFileSaved;

    // Start is called before the first frame update
    void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener (() => saveButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Iniciate (TreeNode file)
    {
        baseFile = file;
        saveButton.interactable = true;
    }

    protected void saveButtonClicked ()
    {
        FileSystem fileSystem = GameState.instance.GetPlayerInfo ().PlayerComputer.FileSystem;
        string path = inputForPathWhereSave.text + "/" + baseFile.Name;

        if (path.NullIfEmpty () == null)
        {
            systemMessageLabel.text = "Path has to be pass to save file!";
        }
        else if (!SystemHelper.CheckIfPathHasCorrectSyntex (inputForPathWhereSave.text))
        {
            systemMessageLabel.text = "Not correct path to directory!";
        }
        else if (fileSystem.FindNode (inputForPathWhereSave.text) != null && !fileSystem.FindNode (inputForPathWhereSave.text).IsDirectory)
        {
            systemMessageLabel.text = "Given path is not a path to direcory!";
        }
        else if (fileSystem.FindNode (path) != null)
        {
            systemMessageLabel.text = "File with this path already exist!";
        }
        else
        {
            TreeNode newFile = fileSystem.CreateNode (path, false);
            newFile.Content = baseFile.Content;
            newFile.WasFileSigned = true;

            systemMessageLabel.text = "File saved!";

            if (OnFileSaved != null)
            {
                OnFileSaved ();
            }

            saveButton.interactable = false;
        }
    }
}
