using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum TaskType
{
    None = 0,
    GetFile,
    InstallProgram,
    DownloadFile,
    GetAccessToStrem,
    CrackPassword,
    LogInToComputer,
    SentFile,
    PutFileOnComuter
}

[Serializable]
public class Task
{
    public int ID;
    public string TaskName { get { return taskName; } }
    public string TaskDescription { get { return taskDescription; } }
    public TaskType TaskType { get; private set; }
    public bool IsDone { get; private set; }

    protected string taskName;
    protected string taskDescription;

    protected TreeNode taskFile;
    protected string taskComputerIP;
    protected TypeOfProgram taskProgram;
    protected string aliciaIP;
    protected string bobIP;

    public Task (TaskData taskData, int id)
    {
        ID = id;
        this.taskName = taskData.Name;
        this.taskDescription = taskData.Description;

        this.TaskType = (TaskType)taskData.Type;
        taskFile = new TreeNode (taskData.File.Name, false, taskData.File.Content);
        taskComputerIP = taskData.ComputerIP;
        taskProgram = (TypeOfProgram)taskData.Program;
        aliciaIP = taskData.A_IP;
        bobIP = taskData.B_IP;
    }

    public void SetTask (TaskType taskType, TreeNode taskFile, string taskComputerIP, TypeOfProgram taskProgram, string aliciaIP, string bobIP)
    {
        TaskType = taskType;
        this.taskFile = taskFile;
        this.taskComputerIP = taskComputerIP;
        this.taskProgram = taskProgram;
        this.aliciaIP = aliciaIP;
        this.bobIP = bobIP;

        CheckIfConditionsMet ();
    }

    public void CheckIfConditionsMet ()
    {
        GameState gameState = GameState.instance;
        if (gameState != null)
        {
            PlayerInfo player = gameState.GetPlayerInfo ();
            FileSystem fileSystem = player?.PlayerComputer.FileSystem;

            if (player != null && fileSystem != null)
            {
                switch (TaskType)
                {
                    case TaskType.GetFile:
                    case TaskType.DownloadFile:
                        if (fileSystem.FileExist (fileSystem.Root, taskFile.Name, taskFile.Content) != null)
                        {
                            IsDone = true;
                        }
                        break;

                    case TaskType.InstallProgram:
                        if (player.ProgramesDownloaded != null && player.ProgramesDownloaded.ContainsKey (taskProgram) && player.ProgramesDownloaded[taskProgram])
                        {
                            IsDone = true;
                        }
                        break;

                    case TaskType.CrackPassword:
                        if (gameState.FindComputerOfIP (taskComputerIP).IsPasswordCracted)
                        {
                            IsDone = true;
                        }
                        break;

                    case TaskType.PutFileOnComuter:
                        if (gameState.FindComputerOfIP (taskComputerIP).FileSystem.FileExist (gameState.FindComputerOfIP (taskComputerIP).FileSystem.Root, taskFile.Name, taskFile.Content) != null)
                        {
                            IsDone = true;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public void CheckIfLogIntoRightComputer (string computerIP)
    {
        if (TaskType == TaskType.LogInToComputer && taskComputerIP == computerIP)
        {
            IsDone = true;
        }
    }
}