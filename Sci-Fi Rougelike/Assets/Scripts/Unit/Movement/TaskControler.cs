//Copyright © Darwin Willers 2017

using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnitMover), typeof(UnitRotater))]
public class TaskControler : MonoBehaviour
{


    public bool TaskInProgress { get; private set; }

    private Queue<Task> _taskQ;

    private UnitMover _unitMover;
    private UnitRotater _unitRotater;
    private Task _currentTask;

    private bool _taskInProg;

    private void Start()
    {
        _taskQ = new Queue<Task>();
        
        _unitMover = GetComponent<UnitMover>();
        _unitRotater = GetComponent<UnitRotater>();

        if (_unitMover != null) _unitMover.TaskDone += TaskDone;
        if (_unitRotater != null) _unitRotater.TaskDone += TaskDone;
    }

    #region AddTask - Functions
    public void AddTask(Task task)
    {
        _taskQ.Enqueue(task);
    }

    public void AddTask(TaskType type, Vector3 pos)
    {
        AddTask( new Task(type, pos));
    }

    public void AddTask(TaskType type, params Vector3[] posArray)
    {
        foreach (var pos in posArray)
        {
            AddTask(new Task(type, pos));
        }
    }
    
    public void AddTask(TaskType type1, TaskType type2, params Vector3[] posArray)
    {
        foreach (var pos in posArray)
        {
            AddTask(new Task(type1, pos));
            AddTask(new Task(type2, pos));
        }
    }
    
    /// <summary>
    /// Automaticly Adds a LookAt and then a Move Task for each position
    /// </summary>
    /// <param name="posArray"></param>
    public void AddTask(params Vector3[] posArray)
    {
        foreach (var pos in posArray)
        {
            AddTask(new Task(TaskType.LookAt, pos));
            AddTask(new Task(TaskType.Move, pos));
        }
    }
    
    
    #endregion

    private void TaskDone()
    {
        _taskInProg = false;
    }

    private void Update()
    {
        if (!_taskInProg && _taskQ.Count != 0)
            StartNextTask();
    }

    private void StartNextTask()
    {
        _currentTask = _taskQ.Dequeue();
        
        switch(_currentTask.Type)
        {
            case TaskType.LookAt:
                _unitRotater.LookAt(_currentTask.Position);
                break;
                
            case TaskType.Move:
                _unitMover.MoveUnit(_currentTask.Position);
                break;
                
            case TaskType.Interact:
                throw new NotImplementedException();               
        }
    }

}