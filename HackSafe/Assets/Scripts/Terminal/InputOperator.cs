using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputOperator : MonoBehaviour
{
    [SerializeField] InputField inputField;

    public delegate void InputHandler (string inutText);
    public event InputHandler OnExchangeGoodsActionDone;


    void Start ()
    {
        inputField.onEndEdit.AddListener (HandleInputOnEnter);
    }

    void HandleInputOnEnter (string userInput)
    {
        if ((Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter)) && OnExchangeGoodsActionDone != null)
        {
            OnExchangeGoodsActionDone (inputField.text);
            inputField.text = string.Empty;
        }
    }

    void HandleInput ()
    {
        string userInput = inputField.text;

        Debug.Log ("User input: " + userInput);

        inputField.text = string.Empty;
    }

    void Update()
    {
        
    }
}
