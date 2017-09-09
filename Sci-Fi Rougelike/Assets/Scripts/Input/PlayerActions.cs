//Copyright © Darwin Willers 2017

using UnityEngine;

public class PlayerActions : MonoBehaviour
{

    private Camera _cam;
    private Player _player;

    private void Start()
    {
        _cam = Camera.main;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveToTileByClick(Input.mousePosition);
        }
    }

    public void MoveToTileByClick(Vector3 pos)
    {
        GroundTile tile;
        if (!Selector.SelectTileByRay(pos, out tile))
            Debug.Log("PLAYERACTIONS - MoveToTileByClick - No Tile was found");
        else
            _player.InteractWith(tile);
    }
}