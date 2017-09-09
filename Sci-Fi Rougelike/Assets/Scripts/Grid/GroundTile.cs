//Copyright © Darwin Willers 2017

using UnityEngine;

public class GroundTile : MonoBehaviour
{

    public Vector3 Position { get; private set; }
    public bool Unwalkable;
    public bool Walkable { get; private set; }
    
    public Transform HostingObject { get; private set; }


    private void Awake()
    {
        Position = transform.position;
        Walkable = !Unwalkable;
    }
    
    private void Start()
    {
      GridManager.Instance.RegisterTile(this);
    }

    public void EnterTile(Transform obj)
    {
        if (HostingObject != null)
            Debug.LogError("COULDNT ENTER TILE: " + obj.name + " couldnt enter tile " +
                           name + " at " + transform.position + "cause tile already hosting "
                           + HostingObject.name);
        else
        {
            HostingObject = obj;
            Walkable = false;
        }
    }

    public void LeaveTile(Transform obj)
    {
        if (obj != HostingObject)
            Debug.LogError("COULDNT LEAVE TILE: Object " + obj.name +
                           " is not the hosted Object at tile" + name + " at "
                           + transform.position + ". Hosted object is " + HostingObject.name);
        else
        {
            HostingObject = null;
            Walkable = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");
        if (HostingObject != null)
            Debug.LogError("COULDNT ENTER TILE: " + other.name + " couldnt enter tile " +
                           name + " at " + transform.position + "cause tile already hosting "
                           + HostingObject.name);
        else
        {
            HostingObject = other.transform;
            Walkable = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit");
        if (other.transform != HostingObject)
            Debug.LogError("COULDNT LEAVE TILE: Object " + other.name +
                           " is not the hosted Object at tile" + name + " at "
                           + transform.position + ". Hosted object is " + HostingObject.name);
        else
        {
            HostingObject = null;
            Walkable = true;
        }
    }
}