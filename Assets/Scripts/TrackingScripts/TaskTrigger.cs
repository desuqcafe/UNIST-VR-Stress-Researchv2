using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTrigger : MonoBehaviour
{
    public TasksManager taskManager;
    public string taskName;

    public void OnSelectDoSomething()
    {
        // Start the task when the object is clicked
        taskManager.StartTask(taskName);
    }
}
