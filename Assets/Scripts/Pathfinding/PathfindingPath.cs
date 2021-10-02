using System.Collections.Generic;
using UnityEngine;

public class PathfindingPath
{
    public List<Vector3> Points;
    
    public PathfindingPath(List<Vector3> controlPoints)
    {
        Points = controlPoints;
    }
}
