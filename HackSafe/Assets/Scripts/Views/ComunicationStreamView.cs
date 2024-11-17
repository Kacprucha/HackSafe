using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComunicationStreamView : MonoBehaviour
{
    [SerializeField] GameObject messageA;
    [SerializeField] GameObject messageB;
    [SerializeField] GameObject content;
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] Text systemMessage;

    protected List<GameObject> messages = new List<GameObject> ();

    protected string aPrefix;
    protected string bPrefix;

    // Start is called before the first frame update
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    private void OnDisable ()
    {
        foreach (GameObject message in this.messages)
            Destroy (message);

        this.messages.Clear ();
    }

    public void IniciateStream (string AName, string BName, List<string> messages)
    {
        foreach (GameObject message in this.messages)
            Destroy (message);

        this.messages.Clear ();
        systemMessage.text = "Procide to encrypt messages";

        aPrefix = " " + AName + ": ";
        bPrefix = " :" + BName + " ";

        StartCoroutine (streamMessages (messages));
    }

    protected IEnumerator streamMessages (List<string> messages)
    {
        bool isA = true;

        foreach (string message in messages)
        {
            GameObject messageObject;

            if (isA)
            {
                messageObject = Instantiate (messageA, content.transform);
                messageObject.GetComponent<Text> ().text = aPrefix + message;
            }
            else
            {
                messageObject = Instantiate (messageB, content.transform);
                messageObject.GetComponent<Text> ().text = message + bPrefix;
            }

            messageObject.SetActive (true);

            Canvas.ForceUpdateCanvases ();
            scrollRect.verticalNormalizedPosition = 0f;

            this.messages.Add (messageObject);

            yield return new WaitForSeconds (3);

            isA = !isA;
        }

        systemMessage.text = "All messages encrypted";
    }
}
