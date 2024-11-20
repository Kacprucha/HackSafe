using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForFNSView : MonoBehaviour
{
    [SerializeField] BlinkingElement transferFromeYToN;
    [SerializeField] BlinkingElement transferFromeNToY;

    protected float blinkInterval = 5f;

    public delegate void FileRecivedHandler ();
    public event FileRecivedHandler OnFileRecived;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable ()
    {
        StartCoroutine (startSendingProcess ());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator startSendingProcess ()
    {
        transferFromeYToN.StartBlinking ();

        yield return new WaitForSeconds (blinkInterval);

        transferFromeYToN.StopBlinking ();

        yield return new WaitForSeconds (blinkInterval / 2);

        transferFromeNToY.StartBlinking ();

        yield return new WaitForSeconds (blinkInterval);

        transferFromeNToY.StopBlinking ();

        if (OnFileRecived != null)
        {
            OnFileRecived ();
        }
    }
}
