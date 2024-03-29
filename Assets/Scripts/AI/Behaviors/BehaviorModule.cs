using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Parent class for behaviors. A behavior holds the logic for executing each task. Each behavior decomposes tasks into a movement and an action.
 */ 
public abstract class BehaviorModule : MonoBehaviour
{
    private List<TaskType> acceptedTasks;
    protected Task assignedTask { get; private set; }

    private void Awake()
    {
        Assert.IsTrue(acceptedTasks != null && acceptedTasks.Count > 0);
    }

    public void AssignNewTask(Task task, MovementModule movementModule)
    {
        if (!acceptedTasks.Contains(task.type))
        {
            Debug.LogError(string.Format($"Attempting to assign wrong task ({task.type}) to behavior {this.name}"));
            return;
        }
        task.status = Status.ToBeProgrammed;
        assignedTask = task;
        InitMovement(task, movementModule);
    }

    private bool InitMovement(Task task, MovementModule movementModule)
    {
        if (task.location == null)
        {
            Debug.LogError(string.Format($"Monster ({movementModule.currentCell}) attempting attack on null cell"));
            return false;
        }

        // check if the destination is accessible

        movementModule.SetDestination(task.location.coordinates);
        movementModule.OnArrival += InitAction;
        return true;
    }

    protected abstract void ExecuteMovement(Task task);
    protected abstract void InitAction(Task task);
    protected abstract void ExecuteAction(Task task);
    public bool IsIdle() { return assignedTask == null; }
    public bool GeneratesOwnTasks => this is ITaskAutoGeneration;
    public List<TaskType> GetAcceptedTasks() { return acceptedTasks; }
}
