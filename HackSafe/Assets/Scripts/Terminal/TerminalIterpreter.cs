using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalIterpreter : MonoBehaviour
{
    [SerializeField] InputOperator playerInputHandler;
    [SerializeField] GameObject playerCommandPrefab;
    [SerializeField] GameObject terminalResponsePrefab;

    protected FILOGueue<GameObject> queue = new FILOGueue<GameObject> (18);

    void Start()
    {
        if (playerInputHandler != null)
        {
            playerInputHandler.OnExchangeGoodsActionDone += HandleInput;
        }
    }

    void HandleInput (string inputText)
    {
        Debug.Log ("User input: " + inputText);

        GameObject newPlayerCommaned = Instantiate (playerCommandPrefab);
        newPlayerCommaned.GetComponent<PassiveTerminalElement> ().UpdateText (inputText);
        newPlayerCommaned.transform.SetParent (this.gameObject.transform);
        newPlayerCommaned.transform.localScale = new Vector3 (1, 1, 1);

        GameObject newTerminalResponse = Instantiate (terminalResponsePrefab);
        newTerminalResponse.GetComponent<PassiveTerminalElement> ().UpdateText (inputText + ": command not found");
        newTerminalResponse.transform.SetParent (this.gameObject.transform);
        newTerminalResponse.transform.localScale = new Vector3 (1, 1, 1);

        if (queue.Count == 18)
        {
            Destroy (queue.Pop ());
            Destroy (queue.Pop ());
        }

        queue.Push (newPlayerCommaned);
        queue.Push (newTerminalResponse);
    }

    void Update()
    {
        
    }
}
