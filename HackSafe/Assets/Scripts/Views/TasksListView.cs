using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TasksListView : MonoBehaviour
{
    [SerializeField] GameObject TaskElemet;
    [SerializeField] GameObject TaksContent;
    [SerializeField] GameObject Separator;
    [SerializeField] GameObject TaskList;

    protected List<TaskView> tasksViews = new List<TaskView> ();
    protected List<Task> tasksData = new List<Task> ();
    protected bool questMarkedAsDone = false;

    public delegate void QuestTasksDoneHandler (int questID);
    public event QuestTasksDoneHandler OnQuestDone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.Instance != null && GameState.Instance.ActiveQuest != null)
        {
            if (tasksData.Count > 0)
            {
                foreach (Task task in tasksData)
                {
                    task.CheckIfConditionsMet ();
                    if (task.IsDone)
                    {
                        tasksViews[task.ID].MarkAsDone ();
                    }
                }
            }

            if (GameState.Instance.ActiveQuest.ChceckIfQuestIsFinished () && !questMarkedAsDone && OnQuestDone != null)
            {
                OnQuestDone (GameState.Instance.ActiveQuest.ID);
                questMarkedAsDone = true;
            }
        }
    }

    public void CrateTaskElements (List<Task> tasks)
    {
        questMarkedAsDone = false;

        if (this.tasksViews.Count > 0)
        {
            foreach (TaskView task in this.tasksViews)
            {
                task.PrepareForDestroy ();
                Destroy (task.gameObject);
            }
        }

        this.tasksViews.Clear ();
        this.tasksData.Clear ();

        foreach (Task task in tasks)
        {
            GameObject taskElement = Instantiate (TaskElemet, TaskList.transform);
            GameObject taskContent = Instantiate (TaksContent, TaskList.transform);
            GameObject separator = Instantiate (Separator, TaskList.transform);

            taskContent.GetComponent<Text> ().text = task.TaskDescription;

            TaskView taskView = taskElement.GetComponent<TaskView> ();
            taskView.Inicialize (task.ID, taskContent.GetComponent<Text> (), TaskList.GetComponent<RectTransform> (), task.TaskName, separator);

            separator.SetActive (true);

            this.tasksViews.Add (taskView);
            this.tasksData.Add (task);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate (TaskList.GetComponent<RectTransform> ());
    }
}
