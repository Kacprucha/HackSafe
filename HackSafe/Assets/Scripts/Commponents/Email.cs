using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Email : MonoBehaviour
{
    [SerializeField] Button mainCommponentButton;

    [SerializeField] Image emailRead;
    [SerializeField] Image emailNotRead;

    [SerializeField] Text title;
    [SerializeField] Text emial;
    [SerializeField] Text time;

    protected bool emialRead;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicialize (string title, string emial, string time, bool read)
    {
        this.title.text = title;
        this.emial.text = emial;
        this.time.text = time;

        emialRead = read;

        if (emialRead)
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
        emialRead = wasReaded;

        if (emialRead)
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
