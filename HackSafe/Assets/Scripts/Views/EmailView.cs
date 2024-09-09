using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailView : MonoBehaviour
{
    [SerializeField] Button mainCommponentButton;

    [SerializeField] Image emailRead;
    [SerializeField] Image emailNotRead;

    [SerializeField] Text title;
    [SerializeField] Text emial;
    [SerializeField] Text time;

    protected Email email;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicialize (Email email)
    {
        this.email = email;
        this.title.text = email.Title;
        this.emial.text = email.EmailAdress;
        this.time.text = email.Time;

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
}
