//Copyright © Darwin Willers 2017

using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(TaskControler))]
public class UnitMover : MonoBehaviour
{
    public Action TaskDone;
                        
    public float moveTime = 1f;

    public bool IsMoving { get; private set; }

    /// <summary>
    /// Only use Vector3 Forward, Backward, Left and Right
    /// </summary>
    /// <param name="dir"></param>
    public void MoveUnit(Vector3 destination)
    {
        if (CheckIsMoving()) return;
        
        //TODO - Look at destination first

        StartCoroutine(MoveToDestination(transform, destination, moveTime));
    }

    private bool CheckIsMoving()
    {
        if (!IsMoving) return false;
        
        Debug.LogError("UnitMover of: " + gameObject.name + " recieved staggerd MoveUnit calls");
        return true;
    }   

    private IEnumerator MoveToDestination(Transform unitTransform, Vector3 destination, float time)
    {
        IsMoving = true;
        Vector3 startPos = unitTransform.position;
        float timePassed = 0f;

        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            unitTransform.position = Vector3.Lerp(startPos, destination, timePassed / time);
            yield return null;
        }
        IsMoving = false;
        if (TaskDone != null) TaskDone();

    }
    
}