//Copyright © Darwin Willers 2017

using UnityEngine;

[RequireComponent(typeof(TaskControler))]
public class Player : MonoBehaviour
{

    public static Transform player { get; private set; }
    private static Vector3 _worldPos;
    private static TaskControler _taskControler;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(transform == null)
            Debug.LogError("Couldnt find a Player Object");

        DontDestroyOnLoad(player);

        _taskControler = GetComponent<TaskControler>();
    }

    public static void PlayerInteractWith(GroundTile tile)
    {
        if(tile.HostingObject != null)
            Debug.Log("Interacting with " + tile.HostingObject.name);
        else if(tile.Walkable)
            _taskControler.AddTask(tile.Position);
        else
            Debug.Log("Nothing to do with " + tile.name);
    }
}