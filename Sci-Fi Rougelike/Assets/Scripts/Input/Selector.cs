#region

using UnityEngine;

#endregion

public static class Selector
{
    private const float MaxRange = 1000f;


    public static bool SelectTileByRay(Vector3 pos, out GroundTile tile)
    {
        tile = null;
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(pos);
        var hits = Physics.RaycastAll(ray, MaxRange);

        foreach (var hit in hits)
        {
            if (!hit.transform.CompareTag("GroundTile")) continue;
            tile = hit.transform.GetComponent<GroundTile>();
            if (tile != null) break;
        }
        

        return tile != null;
    }
}