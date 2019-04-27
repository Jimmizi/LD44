using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{

    [SerializeField, Range(0f, 2f)] public float speed;
    private Pathfinding.PathfinderManager _pathfinderManager;
    private Vector3 _target;
    private List<Node> _path;
    private int _ix;
    
    void Start()
    {
        transform.position = new Vector3(-2, -4, 0);
        _pathfinderManager = PathfinderManager.GetInstance();
        _target = new Vector3(6, 6, 0);
        _ix = 0;
    }

    private void PathCallback(List<Node> path)
    {
        if (path != null)
        {
            //_target = path[0].position;
            _path = path;
            _ix = 0;
        }    
    }
    
    void Update()
    {
        if (_path == null) return;
        
        if ((_path[_ix].position - transform.position).magnitude < 1.2f) _ix++;
        if (_ix >= _path.Count) _ix = _path.Count - 1;

        var direction = _path[_ix].position - transform.position;

        /*if (direction != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2.0f);
        }*/

        var velocity = speed * Time.deltaTime;
        transform.position += direction * velocity;
    }

    private void FixedUpdate()
    {
        if ((transform.position - _target).magnitude < 1.0f)
        {
            return;
        }
        
        _pathfinderManager.RequestPathfind(transform.position, _target, PathCallback);
    }
}
