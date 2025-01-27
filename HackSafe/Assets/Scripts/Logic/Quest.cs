using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionsAfterRecivingEmail
{
    None = 0,
    EnablePrograms,
    AddComputersToNetwork,
    DelateComputers,
    LoadNetworkView
}

public class Quest
{
    public int ID { get; private set; }
    public EmailData EmailData { get { return email; } }
    public List<ActionsAfterRecivingEmail> AfterEmailActions { get { return actionAfterRecivingEmail; } }
    public List<Task> Tasks { get { return tasks; } }

    public TreeNode FileToSent { get { return fileToSent; } }
    public TreeNode FileToReceive { get { return fileToReceive; } }

    protected TaskType finalCondition;
    protected List <Task> tasks = new List <Task> ();
    protected EmailData email;

    protected List<ActionsAfterRecivingEmail> actionAfterRecivingEmail;
    protected List<TypeOfProgram> typeOfPrograms = new List<TypeOfProgram> ();
    protected List<Computer> comperToAddToNetwork;
    protected List<string> comperIPsToDelete;
    protected TreeNode fileToSent;
    protected TreeNode fileToReceive;
    protected string ipWhereFileShouldBe;

    public Quest (int id, TaskType finalCondition, EmailData email, List<Task> tasks, 
                    List<ActionsAfterRecivingEmail> actionAfterRecivingEmail, List<TypeOfProgram> typeOfPrograms, List<Computer> computers, List<string> computersIPs,
                    TreeNode fileToSent, TreeNode fileToRecive, string computerIP)
    {
        this.ID = id;
        this.finalCondition = finalCondition;
        this.email = email;
        this.tasks = tasks;

        this.actionAfterRecivingEmail = actionAfterRecivingEmail;
        this.typeOfPrograms = typeOfPrograms;
        this.comperToAddToNetwork = computers;
        this.comperIPsToDelete = computersIPs;
        this.fileToSent = fileToSent;
        this.fileToReceive = fileToRecive;
        this.ipWhereFileShouldBe = computerIP;
    }

    public void MakeActionAfterRecivingEmail ()
    {
        GameState gameState = GameState.Instance;
        PlayerInfo playerInfo = gameState.GetPlayerInfo ();

        foreach (ActionsAfterRecivingEmail action in actionAfterRecivingEmail)
        {
            switch (action)
            {
                case ActionsAfterRecivingEmail.EnablePrograms:
                    foreach (TypeOfProgram program in typeOfPrograms)
                    {
                        playerInfo.ProgramesAllowedToDownload[program] = true;
                    }
                    break;

                case ActionsAfterRecivingEmail.AddComputersToNetwork:
                    foreach (Computer computer in comperToAddToNetwork)
                    {
                        gameState.ComapnysComputers.Add (computer);
                    }
                    break;

                case ActionsAfterRecivingEmail.DelateComputers:
                    foreach (string ip in comperIPsToDelete)
                    {
                        Computer computer = gameState.FindComputerOfIP (ip);
                        if (computer != null)
                        {
                            gameState.ComapnysComputers.Remove (computer);
                        }
                    }
                    break;
            }
        }
    }

    public bool ChceckIfQuestIsFinished ()
    {
        bool result = false;

        if (tasks != null && tasks.Count > 0)
        {
            foreach (Task task in tasks)
            {
                if (task.IsDone)
                {
                    result = true;
                }
                else
                {
                    result = false;
                    break;
                }
            }
        }
        else
        {
            result = true;
        }

        switch (finalCondition)
        {
            case TaskType.SentFile:
                result = false;
                break;
        }

        return result;
    }
}
