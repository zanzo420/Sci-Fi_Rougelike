#region

using System;
using UnityEngine;

#endregion

[Serializable]
public enum TaskType
{
    Move,
    LookAt,
    Interact
}

public struct Task
{
    public TaskType type;
    public Vector3 position;

    public Task(TaskType type, Vector3 pos)
    {
        this.type = type;
        position = pos;
    }
}