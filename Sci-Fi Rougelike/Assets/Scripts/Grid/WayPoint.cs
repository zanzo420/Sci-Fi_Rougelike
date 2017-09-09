#region

using System;
using UnityEngine;

#endregion

public class WayPoint : IComparable
{
    public WayPoint formerWayPoint;
    public int gapSize;

    public GroundTile tile;
    public int wayCost;

    public WayPoint(GroundTile tile, int wayCost, int gapSize)
    {
        
        this.tile = tile;
        formerWayPoint = null;
        this.wayCost = wayCost;
        this.gapSize= gapSize;
    }

    public WayPoint(GroundTile tile, WayPoint formerWayPoint, int wayCost, int gapSize)
    {
        
        this.tile = tile;
        this.formerWayPoint = formerWayPoint;
        this.wayCost = wayCost;
        this.gapSize= gapSize;
    }

    public int Potential
    {
        get { return wayCost + gapSize; }
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        var other = obj as WayPoint;

        if (other != null)
            return Potential.CompareTo(other.Potential);

        Debug.LogError("Cant compare a WayPoint to a non WayPoint");
        return 0;

    }
}