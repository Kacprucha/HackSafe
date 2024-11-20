using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CaptureFileView : MonoBehaviour
{
    [SerializeField] Text fileNameLabel;
    [SerializeField] InputField playerInputField;
    [SerializeField] Button sentButton;

    [SerializeField] Text systemMessageLabel;

    public delegate void FileSentHandler (TreeNode file);
    public event FileSentHandler OnFileSent;

    // Start is called before the first frame update
    void Start()
    {
        if (sentButton != null)
        {
            sentButton.onClick.AddListener (() => onSentButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Iniciate (TreeNode file)
    {
        Computer playerComputer = GameState.instance.GetPlayerInfo ().PlayerComputer;

        TreeNode newNode = playerComputer.FileSystem.CreateNode ($"/home/{playerComputer.Username.ToLower ()}/documents/{file.Name}", false);
        newNode.Content = file.Content;

        fileNameLabel.text = playerComputer.FileSystem.GetPath (newNode);

        sentButton.interactable = true;
    }

    protected void onSentButtonClicked ()
    {
        FileSystem fileSystem = GameState.instance.GetPlayerInfo ().PlayerComputer.FileSystem;
        string path = playerInputField.text;

        if (path.NullIfEmpty () == null)
        {
            systemMessageLabel.text = "Path has to be pass to save file!";
        }
        else if (!SystemHelper.CheckIfPathHasCorrectSyntex (path))
        {
            systemMessageLabel.text = "Not correct path to file!";
        }
        else if (fileSystem.FindNode (path) == null)
        {
            systemMessageLabel.text = "File does not exist!";
        }
        else
        {
            TreeNode newFile = fileSystem.FindNode (path);

            if (OnFileSent != null)
            {
                OnFileSent (newFile);
            }

            sentButton.interactable = false;
        }
    }
}
