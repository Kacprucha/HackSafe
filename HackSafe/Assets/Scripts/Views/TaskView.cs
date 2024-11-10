using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskView : MonoBehaviour
{
    [SerializeField] Text Title;
    [SerializeField] Image DoneMarker;
    [SerializeField] Button RollDownButton;
    [SerializeField] Image ButtonImage;

    [SerializeField] Text Description;
    [SerializeField] RectTransform TasksContent;

    protected int taskRelatedId;
    protected bool isRolledDown = false;

    // Start is called before the first frame update
    void Start()
    {
        if (RollDownButton != null)
        {
            RollDownButton.onClick.AddListener (onRollDwonButtonClicked);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Inicialize (int taskId, Text description, RectTransform tasksList, string title)
    {
        taskRelatedId = taskId;
        Description = description;
        TasksContent = tasksList;

        Title.text = title;
    }

    public void PrepareForDestroy ()
    {
        Destroy (Description.gameObject);
    }

    public void MarkAsDone ()
    {
        RollDownButton.gameObject.SetActive (false);
        DoneMarker.gameObject.SetActive (true);

        isRolledDown = true;
        onRollDwonButtonClicked ();
    }

    protected void onRollDwonButtonClicked ()
    {
        if (isRolledDown)
        {
            isRolledDown = false;

            Description.gameObject.SetActive (false);
            ButtonImage.transform.localEulerAngles = new Vector3 (0, 0, 0f);

            LayoutRebuilder.ForceRebuildLayoutImmediate (TasksContent);
        }
        else
        {
            isRolledDown = true;

            Description.gameObject.SetActive (true);
            ButtonImage.transform.localEulerAngles = new Vector3 (0, 0, 90f);

            LayoutRebuilder.ForceRebuildLayoutImmediate (TasksContent);
        }
    }
}
