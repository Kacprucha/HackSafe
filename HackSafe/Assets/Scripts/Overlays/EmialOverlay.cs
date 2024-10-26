using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmialOverlay : DraggableOverlay
{
    [SerializeField] Button atachmentButton;
    [SerializeField] Button sentButton;

    [SerializeField] GameObject emailViewPrefab;
    [SerializeField] GameObject scrollViewContent;

    [SerializeField] Text emailContentSubject;
    [SerializeField] Text emailContentAdress;
    [SerializeField] Text emailContentLetter;

    protected List<EmailView> emailViews = new List<EmailView>();

    public delegate void SetEmailViewButtonHandler (int emialID, bool emailRead);
    public event SetEmailViewButtonHandler OnSetEmailViewButtonClicked;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start ();
    }

    private void OnEnable ()
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
                }
            }
        }
    }

    private void OnDisable ()
    {
        foreach (EmailView emailView in emailViews)
        {
            emailView.OnMainComponentButtonClicked -= emailViewButtonClicked;
        }

        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy (child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
