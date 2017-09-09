//Copyright © Darwin Willers 2017

using System;
using System.Collections;
using UnityEngine;


public class UnitMover : MonoBehaviour
{
    public Action taskDone;
                        
    public float moveTime = 1f;

    public bool IsMoving { get; private set; }

    
    public void MoveUnit(Vector3 destination)
    {
        if (CheckIsMoving() || !CanMoveToDestination(destination)) return;
        
        StartCoroutine(MoveToDestination(destination, moveTime));
    }

    private bool CheckIsMoving()
    {
        if (!IsMoving) return false;
        
        Debug.LogError("UnitMover of: " + name + " recieved staggerd MoveUnit calls");
        return true;
    }   

    private IEnumerator MoveToDestination(Vector3 destination, float time)
    {
        IsMoving = true;
        var startPos = transform.position;
        var timePassed = 0f;


        while (timePassed < time)
        {
            timePassed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, destination, timePassed / time);
            yield return null;
        }
        
        transform.position = destination;
        IsMoving = false;
        if (taskDone != null) taskDone();

    }

    private bool CanMoveToDestination(Vector3 destination)
    {
        GroundTile tile;
        if (!GridManager.Instance.GetTile(destination, out tile))
        {
            Debug.LogError("UNITMOVER of " + name + "cant move to destination " + destination
                           + "because no GroundTile was found there");
            return false;
        }

        if (tile.Walkable) return true;
        
        Debug.LogError("UNITMOVER of " + name + " was trying to move to the not walkable gridtile"
                       + tile.name + " at " + tile.Position);
        return false;
    }

    
    
}