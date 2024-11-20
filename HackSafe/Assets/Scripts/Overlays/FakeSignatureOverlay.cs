using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSignatureOverlay : DraggableOverlay
{
    [SerializeField] GenerateFileForFNSView generateFileView;
    [SerializeField] WaitingForFNSView waitingView;
    [SerializeField] SavingFileForFNSView savingFileView;

    protected TreeNode fileToSign;

    public override void ShowOverlay ()
    {
        base.ShowOverlay ();

        generateFileView.gameObject.SetActive (true);
        waitingView.gameObject.SetActive (false);
        savingFileView.gameObject.SetActive (false);
    }

    private void OnEnable ()
    {
        generateFileView.OnSentFile += showWaitingView;
        waitingView.OnFileRecived += showSavingView;
        savingFileView.OnFileSaved += fileSaved;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable ()
    {
        fileToSign = null;

        generateFileView.OnSentFile -= showWaitingView;
        waitingView.OnFileRecived -= showSavingView;
        savingFileView.OnFileSaved -= fileSaved;
    }

    protected void showWaitingView (TreeNode file)
    {
        fileToSign = file;
        generateFileView.gameObject.SetActive (false);
        waitingView.gameObject.SetActive (true);
    }

    protected void showSavingView ()
    {
        waitingView.gameObject.SetActive (false);
        savingFileView.gameObject.SetActive (true);
        savingFileView.Iniciate (fileToSign);
    }

    protected void fileSaved ()
    {
        
    }
}
