//Copyright © Darwin Willers 2017

using UnityEngine;
using System.Collections.Generic;


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
    
    


}