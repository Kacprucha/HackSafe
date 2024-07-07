using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUserOverlay : MonoBehaviour
{
    [SerializeField] InputField usernameInputField;
    [SerializeField] InputField passwordInputField;
    [SerializeField] Button saveButton;

    public delegate void SaveButtonRegisterUserOverlayHandler (string username, string password);
    public event SaveButtonRegisterUserOverlayHandler OnSaveButtonClicked;

    void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener (() => SaveButtonClicked());
        }
    }

    void SaveButtonClicked ()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        OnSaveButtonClicked (username, password);
    }
}
