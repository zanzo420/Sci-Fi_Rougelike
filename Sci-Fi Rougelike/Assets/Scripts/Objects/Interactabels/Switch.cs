#region

using System;
using UnityEngine;

#endregion

public class Switch : Interactabel
{
    private bool _status;

    public Action<bool> switchActivated;

    public override void Interact(Transform other)
    {
        Debug.Log(other.name + " interacted with " + name);
        
        _status = !_status;

        if (switchActivated != null)
            switchActivated(_status);
    }
}