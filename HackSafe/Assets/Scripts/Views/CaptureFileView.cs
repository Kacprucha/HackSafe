using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureFileView : MonoBehaviour
{
    [SerializeField] Text fileNameLabel;
    [SerializeField] InputField playerInputField;
    [SerializeField] Button sentButton;

    [SerializeField] Text systemMessage;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
