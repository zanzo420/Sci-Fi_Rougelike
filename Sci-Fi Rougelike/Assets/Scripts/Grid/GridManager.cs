//Copyright © Darwin Willers 2017

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    private Dictionary<Vector3, GroundTile> _grid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if(Instance != this) DestroyImmediate(this);
        
        _grid = new Dictionary<Vector3, GroundTile>();        
    }

    public void RegisterTile(GroundTile tile)
    {
        if(!_grid.ContainsKey(tile.Position))
            _grid.Add(tile.Position, tile);
        else
        if(_grid[tile.Position] == tile)
            Debug.LogError("COULDNT REGISTER TILE: Tile " + tile.name + " is already registered");
        else
            Debug.LogError("COULDNT REGISTER TILE: Tile " + tile.name +
                           "couldnt be registerd because tile" + _grid[tile.Position].name +
                           " is alread registered at position" + tile.Position);
                            
    }

    public void RemoveTile(GroundTile tile)
    {
        if(_grid.ContainsKey(tile.Position))
            _grid.Remove(tile.Position);
        else
            Debug.LogError("COULDNT REMOVE TILE: Tile at Position " + tile.Position + " not found.");
    }

    public bool GetTile(Vector3 pos, out GroundTile tile)
    {
        tile = null;

        if (_grid.ContainsKey(pos))
            tile = _grid[pos];

        return tile != null;
    }

    public bool FindPath(Vector3 start, Vector3 end, int maxStamina, out Stack<GroundTile> tiles)
    {
        tiles = new Stack<GroundTile>();

        if (!_grid.ContainsKey(end))
        {
            Debug.LogError("Pathfinding could find destination");
            return false;
        }

        var openList = new List<WayPoint>();
        var closedList = new Dictionary<Vector3, int>();

        var startGap = GetGap(start, end);
        if (startGap > maxStamina)
        {
            Debug.Log("Destination to far away");
            return false;
        }

        if (!_grid.ContainsKey(start))
        {
           Debug.LogError("PATHFINDIG: Start Position was not found");
            return false;
        }
        openList.Add(new WayPoint(_grid[start], 0, startGap));

        WayPoint current = null;

        while (openList.Count != 0)
        {
            current = openList.First();

            if (current.tile.Position == end) break;
            
            

            #region AddNeighbourTiles

            if (current.wayCost < maxStamina)
            {
                for (var x = -1; x <= 1; x++)
                {
                    for (var z = -1; z <= 1; z++)
                    {
                        if ((x != 0 || z == 0) && (z != 0 || x == 0)) continue;
                        var newPos = current.tile.Position + new Vector3(x, 0, z);
                        if (!_grid.ContainsKey(newPos)) continue;
                        if (!_grid[newPos].Walkable && newPos != end) continue;
                        var wp = new WayPoint(_grid[newPos], current,
                            current.wayCost + 1, GetGap(newPos, end));
                        if (!closedList.ContainsKey(wp.tile.Position))
                            openList.Add(wp);
                        else if (closedList[wp.tile.Position] > wp.Potential)
                            openList.Add(wp);
                    }
                }
            }

            #endregion

            if(!openList.Remove(current))
                Debug.LogError("PATHFINDING: Couldnt remove current tile from openList");
            
            if(!closedList.ContainsKey(current.tile.Position))
            closedList.Add(current.tile.Position, current.Potential);
            else if (closedList[current.tile.Position] > current.Potential)
            {
                closedList.Remove(current.tile.Position);
                closedList.Add(current.tile.Position, current.Potential);                
            }
            
            openList.Sort();

        }

        if (current == null)
        {
            Debug.LogError("PATHFINDING: Current tile was NULL");
            return false;
        }

        if (current.tile.Position != end)
        {
            Debug.Log("Couldnt reach tile");
            return false;
        }

        if (current.wayCost > maxStamina)
        {
            Debug.LogError("Found a PAth but cost to much Stamina");
            return false;
        }

        while (current.formerWayPoint != null)
        {
            tiles.Push(current.tile);
            current = current.formerWayPoint;
        } 

        return true;




    }

    private static int GetGap(Vector3 a, Vector3 b)
        {
            var gap = b - a;
            return (int) (Mathf.Abs(gap.x) + Mathf.Abs(gap.z));
        }
    }