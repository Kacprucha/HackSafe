using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputOperator : MonoBehaviour
{
    [SerializeField] InputField inputField;

    public delegate void InputHandler (string inutText);
    public event InputHandler OnInputEntered;


    void Start ()
    {
        inputField.onEndEdit.AddListener (HandleInputOnEnter);
    }

    void Update ()
    {

    }

    public void ChangeIteractibilityOfInputField (bool value)
    {
        inputField.interactable = value;
    }

    public void ChangeColourOfText (bool black)
    {
        Color color = black ? new Color (0,0,0) : new Color (0.2588235f, 1, 0);
        inputField.textComponent.color = color;
    }

    public void ActiveInputField ()
    {
        inputField.ActivateInputField ();
    }

    void HandleInputOnEnter (string userInput)
    {
        if ((Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) && OnInputEntered != null)
        {
            OnInputEntered (inputField.text);
            inputField.text = string.Empty;
            inputField.ActivateInputField ();
        }
    }

    void HandleInput ()
    {
        string userInput = inputField.text;

        Debug.Log ("User input: " + userInput);

        inputField.text = string.Empty;
    }
}
