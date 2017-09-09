//Copyright © Darwin Willers 2017

using System;
using UnityEngine;


public class UnitInteractor : MonoBehaviour
{

    public Action taskDone;

    public void Interact(Vector3 destination)
    {
        GroundTile tile;
        if (!GridManager.Instance.GetTile(destination, out tile))
        {
            Debug.LogError(name + " tried to interact with nothing");
            return;
        }

    var interactabel = tile.HostingObject.GetComponent<Interactabel>();
        if (interactabel != null)
            interactabel.Interact(transform);        
        else
            Debug.Log("Target is no interactabel");

        if (taskDone != null) taskDone();
    }


}