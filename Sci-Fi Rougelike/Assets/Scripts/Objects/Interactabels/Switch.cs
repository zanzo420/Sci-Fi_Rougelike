//Copyright © Darwin Willers 2017

using System;
using UnityEngine;

public class Switch : Interactabel
{

    private bool _status;

    public Action<bool> SwitchActivated;
    
    public override void Interact(Transform other)
    {
        Debug.Log(other.name + " interacted with " + name);
        
        _status = !_status;

        if (SwitchActivated != null)
            SwitchActivated(_status);
    }
    
    


}