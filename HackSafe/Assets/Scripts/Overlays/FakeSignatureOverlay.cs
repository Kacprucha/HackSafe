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

        FakeSignatureLogic.Instance.StartProgram ();
    }

    public override void CloseOverlay ()
    {
        base.CloseOverlay ();

        FakeSignatureLogic.Instance.StopProgram ();
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

        FakeSignatureLogic.Instance.StopProgram ();
    }

    protected void showWaitingView (TreeNode file)
    {
        fileToSign = file;
        generateFileView.gameObject.SetActive (false);
        waitingView.gameObject.SetActive (true);

        FakeSignatureLogic.Instance.UpdateProgram (25, 20, 0);
    }

    protected void showSavingView ()
    {
        waitingView.gameObject.SetActive (false);
        savingFileView.gameObject.SetActive (true);
        savingFileView.Iniciate (fileToSign);
    }

    protected void fileSaved ()
    {
        StartCoroutine (usageForFileSaving ());
    }

    IEnumerator usageForFileSaving ()
    {
        FakeSignatureLogic.Instance.UpdateProgram (10, 10, 30);

        yield return new WaitForSeconds (3f);

        FakeSignatureLogic.Instance.UpdateProgram (10, 10, 0);
    }
}
