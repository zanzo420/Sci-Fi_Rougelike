//Copyright © Darwin Willers 2017

using System;
using System.Collections;
using UnityEngine;


public class UnitRotater : MonoBehaviour
{

    public Action taskDone;
    
    public float rotationTime = .5f;
    public bool IsRotating { get; private set; }

    public void LookAt(Vector3 target)
    {
        if (CheckIsRotating()) return;

        StartCoroutine(LookAt(transform, target, rotationTime));
    }

    private IEnumerator LookAt(Transform unit, Vector3 target, float time)
    {
        IsRotating = true;
        
        float timePassed = 0;
        target.y = unit.position.y;
        var startRotation = unit.rotation;
        unit.LookAt(target);
        var endRotation = unit.rotation;
        unit.rotation = startRotation;

        if (startRotation != endRotation)
        {
            while (timePassed < rotationTime)
            {
                timePassed += Time.deltaTime;
                unit.rotation = Quaternion.Lerp(startRotation, endRotation, timePassed / time);
                yield return null;
            }
            
            unit.rotation = endRotation;
        }

        IsRotating = false;
        if (taskDone != null) taskDone();
    }
    
    private bool CheckIsRotating()
    {
        if (!IsRotating) return false;
        Debug.LogError("UnitRot cator of: " + gameObject.name + " recieved multiple LookAt calls");
        return true;
    }
}