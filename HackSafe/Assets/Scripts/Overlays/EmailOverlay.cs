using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EmailOverlay : DraggableOverlay
{
    [SerializeField] Button atachmentButton;
    [SerializeField] Button sentButton;

    [SerializeField] GameObject emailViewPrefab;
    [SerializeField] GameObject scrollViewContent;

    [SerializeField] Text emailContentSubject;
    [SerializeField] Text emailContentAdress;
    [SerializeField] Text emailContentLetter;

    protected List<EmailView> emailViews = new List<EmailView>();
    protected int numberOfEmailsWhenEnabled = 0;

    public delegate void SetEmailViewButtonHandler (int emialID, bool emailRead);
    public event SetEmailViewButtonHandler OnSetEmailViewButtonClicked;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start ();

        if (atachmentButton != null)
        {
            atachmentButton.onClick.AddListener (onAtachmentButtonClicked);
        }
    }

    private void OnEnable ()
    {
        loadRecivedEmails ();
        numberOfEmailsWhenEnabled = emailViews.Count;
    }

    private void OnDisable ()
    {
        destroyRecivedEmails ();
    }

    // Update is called once per frame
    void Update()
    {
        GameState gameState = GameState.instance;

        if (gameState != null && gameState.GetPlayerInfo () != null)
        {
            if (numberOfEmailsWhenEnabled > 0 && gameState.GetPlayerInfo ().RecivedEmails.Count > numberOfEmailsWhenEnabled)
            {
                destroyRecivedEmails ();
                loadRecivedEmails ();
                numberOfEmailsWhenEnabled = emailViews.Count;
            }
        }
    }

    protected void loadRecivedEmails ()
    {
        GameState gameState = GameState.instance;

        if (gameState != null && gameState.GetPlayerInfo () != null)
        {
            PlayerInfo player = gameState.GetPlayerInfo ();

            List<Email> emails = player.RecivedEmails;

            if (emails != null && emails.Count > 0)
            {
                foreach (Email email in emails)
                {
                    GameObject emailView = Instantiate (emailViewPrefab, scrollViewContent.transform);
                    EmailView emailViewComponent = emailView.GetComponent<EmailView> ();
                    emailViews.Add (emailViewComponent);

                    if (emailViewComponent != null)
                    {
                        emailViewComponent.Inicialize (email);
                        emailViewComponent.OnMainComponentButtonClicked += emailViewButtonClicked;
                    }

                    emailView.SetActive (true);
                }
            }
        }
    }

    protected void destroyRecivedEmails ()
    {
        List<EmailView> temp = new List<EmailView> ();
        temp = emailViews.GetRange (0, emailViews.Count);

        emailViews.Clear ();

        foreach (EmailView emailView in temp)
        {
            emailView.OnMainComponentButtonClicked -= emailViewButtonClicked;
            Destroy (emailView.gameObject);
        }
    }

    protected void emailViewButtonClicked (Email email)
    {
        if (emailContentSubject != null && emailContentAdress != null && emailContentLetter != null)
        {
            emailContentSubject.text = email.Subject;
            emailContentAdress.text = email.EmailAdress;
            emailContentLetter.text = email.Content;
        }

        if (atachmentButton != null)
        {
            atachmentButton.gameObject.SetActive (email.OpenAttachemntButtonNecesery);
        }

        if (sentButton != null)
        {
            sentButton.gameObject.SetActive (email.SentButtonNecesery);
        }

        if (OnSetEmailViewButtonClicked != null)
        {
            OnSetEmailViewButtonClicked (email.Id, email.EmailRead);
        }
    }

    protected void onAtachmentButtonClicked ()
    {
        GameState gameState = GameState.instance;
        FileSystem fileSystem = gameState.GetPlayerInfo ().PlayerComputer.FileSystem;

        if (gameState != null && gameState.ActiveQuest.FileToSent != null)
        {
            if (fileSystem.FileExist (fileSystem.Root, gameState.ActiveQuest.FileToSent.Name, gameState.ActiveQuest.FileToSent.Content) == null)
            {
                fileSystem.CreateNode ($"/home/{gameState.GetPlayerInfo ().PlayerComputer.Username.Replace (" ", "")}/download/{gameState.ActiveQuest.FileToSent.Name}", false).Content = gameState.ActiveQuest.FileToSent.Content;

                atachmentButton.gameObject.SetActive (false);
            }
        }
    }
}
