//Copyright © Darwin Willers 2017

using System;
using Debug = UnityEngine.Debug;

public class WayPoint : IComparable
{

    public GroundTile tile;
    public WayPoint formerWayPoint;
    public int wayCost;
    public int gapSize;

    public int potential
    {
        get { return wayCost + gapSize; }
    }
    
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

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        var other = obj as WayPoint;

        if (other != null)
            return potential.CompareTo(other.potential);

        Debug.LogError("Cant compare a WayPoint to a non WayPoint");
        return 0;

    }


}