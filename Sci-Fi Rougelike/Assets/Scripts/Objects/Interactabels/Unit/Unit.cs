#region

using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof(TaskControler))]
public class Unit : MonoBehaviour
{
    private TaskControler _taskControler;

    public int maxStamina;
    public int Stamina { get; private set; }

    private void Awake()
    {
        _taskControler = GetComponent<TaskControler>();
        if(_taskControler == null)
            Debug.LogError("Unit: " + name + " is missing a TaskControler" );
        Stamina = maxStamina;
    }

    public void UseStamina(int amount = 1)
    {
        if (amount > Stamina)
        {
            Debug.LogError(name + " used more Stamina then it has");
            return;
        }

        Stamina -= amount;
    }

    public void InteractWith(Vector3 pos)
    {
        GroundTile tile;
        if(GridManager.Instance.GetTile(pos, out tile))
            InteractWith(tile);
        else
            Debug.LogError("Unit: " + name + " didnt found a tile at " + pos);
    }

    public void InteractWith(GroundTile tile)
    {
        if (_taskControler.TaskInProgress) return;

        if (tile.HostingObject != null)
        {
            var interactabel = tile.HostingObject.GetComponent<Interactabel>();
            if (interactabel == null) return;
            if (GetGap(transform.position, tile.Position) > 1)
            {
                Debug.Log("Interactabel to far away");
            }
            else
            {
                _taskControler.AddTask(TaskType.LookAt, tile.Position);
                _taskControler.AddTask(TaskType.Interact, tile.Position);
            }
        }
        else if (tile.Walkable)
        {

            Stack<GroundTile> tiles;
            if (!GridManager.Instance.FindPath(transform.position, tile.Position, Stamina,
                out tiles)) return;

            UseStamina(tiles.Count);

            while (tiles.Count > 0)
                _taskControler.AddTask(tiles.Pop().Position);
        }
    }

    private static int GetGap(Vector3 a, Vector3 b)
        {
            var distance = b - a;
            return (int) (Mathf.Abs(distance.x) + Mathf.Abs(distance.z));
        }
}