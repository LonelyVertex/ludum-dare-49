using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModestTree;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private ProBuilderMesh mesh;

    private Vector3[] _midpoints;
    private Dictionary<Vector3, List<Vector3>> _faceNeighbors;

    public void InitializePathfindingGraph()
    {
        GenerateGraph();
    }

    public Vector3 FindClosestPathfindingPoint(Vector3 worldPos)
    {
        var dist = float.MaxValue;
        Vector3 currentMin = default;
        foreach (var midpoint in _midpoints)
        {
            var newDist = Vector3.Distance(midpoint, worldPos);
            if (!(newDist < dist)) continue;
            
            dist = newDist;
            currentMin = midpoint;
        }

        return currentMin;
    }
    
    public PathfindingPath FindPath(Vector3 start, Vector3 goal)
    {
        var closestStartPoint = FindClosestPathfindingPoint(start);
        var closestGoalPoint = FindClosestPathfindingPoint(goal);

        var frontier = new PriorityQueue<Vector3>();
        frontier.Enqueue(closestStartPoint, 0f);

        var cameFrom = new Dictionary<Vector3, Vector3>();
        var costSoFar = new Dictionary<Vector3, float>();

        cameFrom.Add(closestStartPoint, closestStartPoint);
        costSoFar.Add(closestStartPoint, 0f);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.Equals(closestGoalPoint))
            {
                break;
            }

            var neighbors = _faceNeighbors[current];
            foreach (var neighbor in neighbors)
            {
                var newCost = costSoFar[current] + 1;

                if (costSoFar.ContainsKey(neighbor) && !(newCost < costSoFar[neighbor])) continue;
                
                if (costSoFar.ContainsKey(neighbor))
                {
                    costSoFar.Remove(neighbor);
                    cameFrom.Remove(neighbor);
                }

                costSoFar.Add(neighbor, newCost);
                cameFrom.Add(neighbor, current);

                frontier.Enqueue(neighbor, newCost + Vector3.Distance(current, neighbor));
            }
        }

        var path = new List<Vector3> { goal };
        for (var c = closestGoalPoint; !c.Equals(closestStartPoint); c = cameFrom[c])
        {
            if (!cameFrom.ContainsKey(c))
            {
                Debug.Log("CameFrom does not contain current.");
            }
            
            path.Add(c);
        }
        path.Add(start);
        path.Reverse();

        return new PathfindingPath(path);
    }

    protected void Start()
    {
        InitializePathfindingGraph();
    }

    private void GenerateGraph()
    {
        if (_faceNeighbors != null) return;
        
        var getNeighborFaces =
            typeof(ElementSelection).GetMethod(
                "GetNeighborFaces",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                new[] { typeof(ProBuilderMesh), typeof(Edge) },
                null
            );

        _faceNeighbors = new Dictionary<Vector3, List<Vector3>>();
        
        foreach (var face in mesh.faces)
        {
            var midPoint = GetMidPoint(mesh.positions, face);
            
            if (!_faceNeighbors.ContainsKey(midPoint))
            {
                _faceNeighbors[midPoint] = new List<Vector3>();
            }
            
            foreach (var edge in face.edges)
            {
                var result = getNeighborFaces?.Invoke(
                    null,
                    new object[] { mesh, edge }
                ) as List<SimpleTuple<Face, Edge>>;
                
                if (result == null) continue;
                
                foreach (var resultFace in result)
                {
                    if (face == resultFace.item1) continue;

                    var list = _faceNeighbors[midPoint];
                    var resultMidPoint = GetMidPoint(mesh.positions, resultFace.item1);
                    if (!list.Contains(resultMidPoint))
                    {
                        list.Add(resultMidPoint);
                    }
                }
            }
        }

        _midpoints = _faceNeighbors.Keys.ToArray();
    }

    public static Vector3 GetMidPoint(IList<Vector3> positions, Face face)
    {
        Assert.IsEqual(3, face.indexes.Count);

        var pos0 = positions[face.indexes[0]];
        var pos1 = positions[face.indexes[1]];
        var pos2 = positions[face.indexes[2]];
        
        return (pos0 + pos1 + pos2) / 3.0f;
    }
}
