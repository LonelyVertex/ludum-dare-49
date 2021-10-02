using System;
using UnityEngine;

public class PathfindingMotor : MonoBehaviour
{
    [SerializeField] private float speed;
    
    public event Action goalReachedEvent;
    
    private PathfindingPath _path;
    private int _startIndex;
    private float _delta;
    
    public void SetPath(PathfindingPath path)
    {
        _path = path;
        _startIndex = 0;
        _delta = 0;
    }

    protected void Update()
    {
        if (_path == null)
        {
            return;
        }

        var a = _path.Points[_startIndex];
        var b = _path.Points[_startIndex + 1];

        _delta += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(a, b, _delta);

        if (_delta >= 1.0f)
        {
            _delta = 0.0f;
            _startIndex++;
        }

        if (_startIndex == _path.Points.Count - 1)
        {
            enabled = false;
            goalReachedEvent?.Invoke();
        }
    }
}
