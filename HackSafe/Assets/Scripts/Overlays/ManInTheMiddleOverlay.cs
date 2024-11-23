using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public enum KeyAsoscietedSite
{
    Non,
    ComputerA,
    ComputerB,
    Player
}

public struct Key
{
    public string KeyName;
    public bool KeyPublic;
    public string AssociatedComputerIP;
    public string CratorComputerIP;

    public Key (string keyName, bool keyPublic, string associatedComputerIP, string cratorComputerIP)
    {
        KeyName = keyName;
        KeyPublic = keyPublic;
        AssociatedComputerIP = associatedComputerIP;
        CratorComputerIP = cratorComputerIP;
    }
}

public class ManInTheMiddleOverlay : DraggableOverlay
{
    [SerializeField] GameObject systemMessageContainer;
    [SerializeField] GameObject keyGeneratorcontainer;
    [SerializeField] GameObject buttonsContainer;
    [SerializeField] GameObject communicationContainer;
    [SerializeField] GameObject captureFileContainer;

    [SerializeField] Text computerAName;
    [SerializeField] Text computerBName;

    [SerializeField] Text publicAKeyName;
    [SerializeField] Text publicBKeyName;
    [SerializeField] GameObject arrowAToY;
    [SerializeField] GameObject arrowYToB;
    [SerializeField] GameObject arrowBToY;
    [SerializeField] GameObject arrowYToA;
    [SerializeField] DropSlot keySentToB;
    [SerializeField] DropSlot keySentToA;
    [SerializeField] DropSlot privateBKeyInFormula;
    [SerializeField] DropSlot publicBKeyInFormula;
    [SerializeField] DropSlot privateAKeyInFormula;
    [SerializeField] DropSlot publicAKeyInFormula;

    [SerializeField] Text systemMessages;

    [SerializeField] Dropdown dropDownMachineChooser;
    [SerializeField] Button generateKeysButton;
    [SerializeField] List<GameObject> loadingBars;

    [SerializeField] GameObject dragableElementPrefab;
    [SerializeField] GameObject dragableElementsContainer;

    static Color naturalColor = Color.white;
    static Color errorColor = Color.red;

    protected List<Key> allKeys = new List<Key> ();
    protected List<Key> keysInUse = new List<Key> ();

    protected Computer computerA;
    protected Computer computerB;

    protected bool[] areKeysGenerated = new bool[2] { false, false };

    protected float blinkInterval = 15f;

    protected ManInTheMiddleLogic manInTheMiddleLogic;

    protected bool aFormulaOkey = false;
    protected bool bFormulaOkey = false;
    protected bool keyGotSentToA = false;
    protected bool keyGotSentToB = false;

    public delegate void StarTimerHandler (DraggableOverlay overlay, float time);
    public event StarTimerHandler OnStartTimer;

    public delegate void FinishConectingToStreamHandler ();
    public event FinishConectingToStreamHandler OnFinishConectingToStream;

    // Start is called before the first frame update
    override public void Start ()
    {
        base.Start ();

        if (keySentToB != null)
        {
            keySentToB.OnElementDroped += handleElementDrop;
        }

        if (keySentToA != null)
        {
            keySentToA.OnElementDroped += handleElementDrop;
        }

        if (privateBKeyInFormula != null)
        {
            privateBKeyInFormula.OnElementDroped += handleElementDrop;
        }

        if (publicBKeyInFormula != null)
        {
            publicBKeyInFormula.OnElementDroped += handleElementDrop;
        }

        if (privateAKeyInFormula != null)
        {
            privateAKeyInFormula.OnElementDroped += handleElementDrop;
        }

        if (publicAKeyInFormula != null)
        {
            publicAKeyInFormula.OnElementDroped += handleElementDrop;
        }

        if (generateKeysButton != null)
        {
            generateKeysButton.onClick.AddListener (onGenerateKeysButtonClicked);
        }

        if (captureFileContainer != null)
        {
            captureFileContainer.GetComponent<CaptureFileView> ().OnFileSent += onSentFileToB;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (aFormulaOkey && bFormulaOkey && keyGotSentToA && keyGotSentToB)
        {
            if (manInTheMiddleLogic.AsociatedTask != null)
            {
                if (!manInTheMiddleLogic.AsociatedTask.IsDone)
                    manInTheMiddleLogic.MarkTaKaskAsDone ();

                if (OnFinishConectingToStream != null)
                    OnFinishConectingToStream ();

                manInTheMiddleLogic.UpdateProgram (manInTheMiddleLogic.AsociatedTask.Comunication != null && manInTheMiddleLogic.AsociatedTask.Comunication.Count > 0);

                afterSuccesfullAtackActions ();
            }
        }
    }

    private void OnDisable ()
    {
        computerA = null;
        computerB = null;

        allKeys.Clear ();
        keysInUse.Clear ();

        areKeysGenerated[0] = false;
        areKeysGenerated[1] = false;

        aFormulaOkey = false;
        bFormulaOkey = false;
        keyGotSentToA = false;
        keyGotSentToB = false;

        for (int i = 1; i < dragableElementsContainer.transform.childCount; i++)
        {
            Transform child = dragableElementsContainer.transform.GetChild (i);
            Destroy (child.gameObject);
        }

        manInTheMiddleLogic.StopProgram ();
    }

    public void ShowOverlay (Computer compA, Computer compB)
    {
        gameObject.SetActive (true);

        keyGeneratorcontainer.SetActive (true);
        buttonsContainer.SetActive (true);
        communicationContainer.SetActive (false);
        captureFileContainer.SetActive (false);

        Inicialize (compA, compB);
    }

    public override void CloseOverlay ()
    {
        base.CloseOverlay ();

        manInTheMiddleLogic.StopProgram ();
    }

    public void Inicialize (Computer compA, Computer compB)
    {
        manInTheMiddleLogic = ManInTheMiddleLogic.Instance;

        computerA = compA;
        computerB = compB;

        computerAName.text = computerA.Username;
        computerBName.text = computerB.Username;

        dropDownMachineChooser.options.Clear ();
        dropDownMachineChooser.options.Add (new Dropdown.OptionData () { text = computerA.Username });
        dropDownMachineChooser.options.Add (new Dropdown.OptionData () { text = computerB.Username });

        generateKies ();

        StartCoroutine (sendKey (computerA, GameState.instance.GetPlayerInfo ().PlayerComputer, arrowAToY));

        manInTheMiddleLogic.AsociatedTask = GameState.instance.ActiveQuest.Tasks.Find (
                                                                    t => t.TaskType == TaskType.GetAccessToStrem &&
                                                                    t.AIP == compA.IP &&
                                                                    t.BIP == compB.IP
                                                                   );

        if (manInTheMiddleLogic.AsociatedTask?.TimeForTaks > 0 && !manInTheMiddleLogic.AsociatedTask.IsDone)
        {
            OnStartTimer (this, manInTheMiddleLogic.AsociatedTask.TimeForTaks);
        }

        manInTheMiddleLogic.StartProgram ();
    }

    protected void generateKies ()
    {
        Computer playerComputer = GameState.instance.GetPlayerInfo ().PlayerComputer;

        string firstLetterOfA = computerA.Username.Substring (0, 1).ToUpper ();
        string firstLetterOfB = computerB.Username.Substring (0, 1).ToUpper ();

        if (firstLetterOfA == firstLetterOfB)
        {
            firstLetterOfB += "'";
        }

        publicAKeyName.text = "R" + firstLetterOfA;
        publicBKeyName.text = "R" + firstLetterOfB;

        Key key = new Key ("R" + firstLetterOfA, true, computerA.IP, computerA.IP);
        allKeys.Add (key);

        key = new Key ("R" + firstLetterOfB, true, computerB.IP, computerB.IP);
        allKeys.Add (key);

        key = new Key ("rY" + firstLetterOfA, false, computerA.IP, playerComputer.IP);
        allKeys.Add (key);

        key = new Key ("rY" + firstLetterOfB, false, computerB.IP, playerComputer.IP);
        allKeys.Add (key);

        key = new Key ("RY" + firstLetterOfA, true, computerA.IP, playerComputer.IP);
        allKeys.Add (key);

        key = new Key ("RY" + firstLetterOfB, true, computerB.IP, playerComputer.IP);
        allKeys.Add (key);
    }

    protected void handleElementDrop (string keyName, KeyAsoscietedSite keyAsoscietedSite, bool isKeySlot, bool isFromulaSlot)
    {
        Computer computer = null;

        switch (keyAsoscietedSite)
        {
            case KeyAsoscietedSite.ComputerA:
                computer = computerA;
                break;
            case KeyAsoscietedSite.ComputerB:
                computer = computerB;
                break;
            default:
                break;
        }

        if (isKeySlot)
        {
            if (computer != null)
            {
                Key key = keysInUse.Find (k => k.KeyName == keyName);

                if (key.KeyName != null)
                {
                    onKeyDropedIntoSlot (key, computer);
                }
            }
        }
        else if (isFromulaSlot)
        {
            if (computer != null)
            {
                string publicKeyStored = "";
                string privateKeyStored = "";

                switch (keyAsoscietedSite)
                {
                    case KeyAsoscietedSite.ComputerA:

                        publicKeyStored = publicAKeyInFormula.StoredKeyName;
                        privateKeyStored = privateAKeyInFormula.StoredKeyName;

                        break;

                    case KeyAsoscietedSite.ComputerB:

                        publicKeyStored = publicBKeyInFormula.StoredKeyName;
                        privateKeyStored = privateBKeyInFormula.StoredKeyName;

                        break;
                }

                Key publicKey = keysInUse.Find (k => k.KeyName == publicKeyStored);
                Key privateKey = keysInUse.Find (k => k.KeyName == privateKeyStored);

                if (publicKey.KeyName != null && privateKey.KeyName != null)
                    onKeyDropedIntoFormula (publicKey, privateKey, computer);
            }
        }
    }

    protected void onKeyDropedIntoSlot (Key key, Computer computer)
    {
        if (manInTheMiddleLogic.CheckIfKeyCorrect (computer, key, true))
        {
            systemMessages.color = naturalColor;
            systemMessages.text = $"Sending key: {key.KeyName} to computer: {computer.Username}";

            StartCoroutine (sendKey (GameState.instance.GetPlayerInfo ().PlayerComputer, computer, computer.IP == computerA.IP ? arrowYToA : arrowYToB));
        }
        else
        {
            systemMessages.color = errorColor;
            systemMessages.text = $"Key: {key.KeyName} is not correct for computer: {computer.Username}";
        }
    }

    protected void onKeyDropedIntoFormula (Key publicKey, Key privateKey, Computer computer)
    {
        if (manInTheMiddleLogic.CheckIsFormulaCorrect (computer, publicKey, privateKey))
        {
            systemMessages.color = naturalColor;
            systemMessages.text = $"Formula is correct for computer: {computer.Username}";

            if (computer.IP == computerA.IP)
            {
                aFormulaOkey = true;
            }
            else
            {
                bFormulaOkey = true;
            }
        }
        else
        {
            systemMessages.color = errorColor;
            systemMessages.text = $"Formula is not correct for computer: {computer.Username}";
        }
    }

    protected void onGenerateKeysButtonClicked ()
    {
        string computerName = dropDownMachineChooser.options[dropDownMachineChooser.value].text;
        Computer computer = computerName == computerA.Username ? computerA : computerB;
        int idex = computerName == computerA.Username ? 0 : 1;

        if (!areKeysGenerated[idex])
        {
            generateKeysButton.interactable = false;

            List<Key> keys = allKeys.FindAll (k => k.AssociatedComputerIP == computer.IP && k.CratorComputerIP == GameState.instance.GetPlayerInfo ().PlayerComputer.IP);
            StartCoroutine (crateNewKeys (keys));
            areKeysGenerated[idex] = true;
        }
    }

    protected IEnumerator crateNewKeys (List<Key> keys)
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds (Random.Range (0.5f, 1));
            loadingBars[i].SetActive (true);
        }

        foreach (Key key in keys)
        {
            GameObject dragableElement = Instantiate (dragableElementPrefab, dragableElementsContainer.transform);
            dragableElement.GetComponent<Label> ().UpdateLabel (key.KeyName);
            dragableElement.SetActive (true);

            keysInUse.Add (key);
        }

        foreach (GameObject loadingBar in loadingBars)
        {
            loadingBar.SetActive (false);
        }

        generateKeysButton.interactable = true;
    }

    protected IEnumerator sendKey (Computer form, Computer to, GameObject asociatedArrow)
    {
        if (form != GameState.instance.GetPlayerInfo ().PlayerComputer)
        {
            asociatedArrow.GetComponent<BlinkingElement> ().StartBlinking ();

            yield return new WaitForSeconds (blinkInterval);

            asociatedArrow.GetComponent<BlinkingElement> ().StopBlinking ();

            Key key = allKeys.Find (k => k.AssociatedComputerIP == form.IP && k.CratorComputerIP == form.IP);

            GameObject dragableElement = Instantiate (dragableElementPrefab, dragableElementsContainer.transform);
            dragableElement.GetComponent<Label> ().UpdateLabel (key.KeyName);
            dragableElement.SetActive (true);

            keysInUse.Add (key);
        }
        else
        {
            asociatedArrow.GetComponent<BlinkingElement> ().StartBlinking ();

            yield return new WaitForSeconds (blinkInterval);

            asociatedArrow.GetComponent<BlinkingElement> ().StopBlinking ();

            if (to.IP == computerB.IP)
            {
                arrowBToY.GetComponent<BlinkingElement> ().StartBlinking ();

                yield return new WaitForSeconds (blinkInterval);

                arrowBToY.GetComponent<BlinkingElement> ().StopBlinking ();

                Key key = allKeys.Find (k => k.AssociatedComputerIP == to.IP && k.CratorComputerIP == to.IP);

                GameObject dragableElement = Instantiate (dragableElementPrefab, dragableElementsContainer.transform);
                dragableElement.GetComponent<Label> ().UpdateLabel (key.KeyName);
                dragableElement.SetActive (true);

                keysInUse.Add (key);
                keyGotSentToB = true;
            }
            else
            {
                Key AKey = keysInUse.Find (k => k.AssociatedComputerIP == to.IP && k.CratorComputerIP == to.IP);

                if (AKey.KeyName == null)
                {
                    systemMessages.color = errorColor;
                    systemMessages.text = $"{computerA.Username} close communication. Closing program in 5s!";

                    yield return new WaitForSeconds (5f);
                    CloseOverlay ();
                }
                else
                {
                    keyGotSentToA = true;
                }
            }
        }
    }

    protected void afterSuccesfullAtackActions ()
    {
        if (manInTheMiddleLogic.AsociatedTask != null)
        {
            if (manInTheMiddleLogic.AsociatedTask.Comunication != null && manInTheMiddleLogic.AsociatedTask.Comunication.Count > 0)
            {
                communicationContainer.SetActive (true);
                keyGeneratorcontainer.SetActive (false);
                buttonsContainer.SetActive (false);

                communicationContainer.GetComponent<ComunicationStreamView> ().IniciateStream (computerA.Username, computerB.Username, manInTheMiddleLogic.AsociatedTask.Comunication);
            }
            else if (manInTheMiddleLogic.AsociatedTask.TaskFile != null)
            {
                captureFileContainer.SetActive (true);
                keyGeneratorcontainer.SetActive (false);
                buttonsContainer.SetActive (false);

                captureFileContainer.GetComponent<CaptureFileView> ().Iniciate (manInTheMiddleLogic.AsociatedTask.TaskFile);
            }
        }
    }

    protected void onSentFileToB (TreeNode file)
    {
        computerB.FileSystem.CreateNode ($"/home/{computerB.Username.Replace (" ", "")}/download/{file.Name}", false).Content = file.Content;
        systemMessages.text = $"File sent to {computerB.Username}! Attack end with succes!";
    }
}
