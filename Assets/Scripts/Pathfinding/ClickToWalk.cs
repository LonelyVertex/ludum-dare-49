using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToWalk : MonoBehaviour
{
    [SerializeField] private PathfindingAgent agent;

    protected void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }
        
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hit))
        {
            agent.SetDestination(hit.point);
        }
    }
}
