using System;
using UnityEngine;
using Zenject;

public class PathfindingAgent : MonoBehaviour
{
    [SerializeField] private PathfindingMotor motor;

    public event Action goealReachedEvent;
    
    [Inject] private readonly Pathfinding _pathfinding;

    public void SetDestination(Vector3 destination)
    {
        var path = _pathfinding.FindPath(transform.position, destination);

        motor.SetPath(path);
        motor.enabled = true;
    }

    protected void Awake()
    {
        motor.enabled = false;
        motor.goalReachedEvent += HandlePathfindingMotorGoalReached;
    }

    private void HandlePathfindingMotorGoalReached()
    {
        goealReachedEvent?.Invoke();
    }
}
