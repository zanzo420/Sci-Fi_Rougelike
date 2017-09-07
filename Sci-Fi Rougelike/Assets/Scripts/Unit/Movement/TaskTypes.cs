//Copyright © Darwin Willers 2017

using System;
using UnityEngine;

[Serializable]
public enum TaskType
{
    Move,
    LookAt,
    Interact
}

public struct Task
{
    public TaskType Type;
    public Vector3 Position;

    public Task(TaskType type, Vector3 pos)
    {
        Type = type;
        Position = pos;
    }
}