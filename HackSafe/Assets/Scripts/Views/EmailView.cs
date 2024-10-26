using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailView : MonoBehaviour
{
    [SerializeField] Button mainCommponentButton;

    [SerializeField] Image emailRead;
    [SerializeField] Image emailNotRead;

    [SerializeField] Text subject;
    [SerializeField] Text emial;
    [SerializeField] Text time;

    protected Email email;

    public delegate void MainComponentButtonHandler (Email email);
    public event MainComponentButtonHandler OnMainComponentButtonClicked;

    void Start()
    {
        
    }

    private void Awake ()
    {
        if (mainCommponentButton != null)
        {
            mainCommponentButton.onClick.AddListener (() => mainComponentButtonClicked ());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicialize (Email email)
    {
        this.email = email;
        this.subject.text = email.Subject;
        this.emial.text = email.EmailAdress;
        this.time.text = email.Day + " " + email.Time;

        if (email.EmailRead)
        {
            emailNotRead.gameObject.SetActive (false);
            emailRead.gameObject.SetActive (true);
        }
        else
        {
            emailNotRead.gameObject.SetActive (true);
            emailRead.gameObject.SetActive (false);
        }
    }

    public Button MainCommponentButton ()
    {
        return mainCommponentButton;
    }

    public void ChangeIfEmialWadRead (bool wasReaded)
    {
        email.SetEmialRead (wasReaded);

        if (email.EmailRead)
        {
            emailNotRead.gameObject.SetActive (false);
            emailRead.gameObject.SetActive (true);
        }
        else
        {
            emailNotRead.gameObject.SetActive (true);
            emailRead.gameObject.SetActive (false);
        }
    }

    protected void mainComponentButtonClicked ()
    {
        if (OnMainComponentButtonClicked != null)
        {
            OnMainComponentButtonClicked (email);
            ChangeIfEmialWadRead (true);
        }
    }
}
