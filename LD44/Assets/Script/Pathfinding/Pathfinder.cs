using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace Pathfinding
{
    
    /// <summary>
    /// Performs optimal path search algorithm (A*) on a grid
    /// provided by the GridGenerator singleton.
    /// </summary>

    public class Pathfinder
    {

        public Node start;
        public Node target;
        
        public volatile bool done = false;
        
        private GridGenerator _gridGenerator;
        private List<Node> _foundPath;
        
        

        public Pathfinder(Vector3 startPosition, Vector3 targetPosition)
        {
            _gridGenerator = GridGenerator.GetInstance();
            _gridGenerator.startPosition = startPosition;
            _gridGenerator.endPosition = targetPosition;
            
            start = _gridGenerator.NodeFromWorldPosition(startPosition);
            target = _gridGenerator.NodeFromWorldPosition(targetPosition);
        }

        public void FindPath()
        {
            _foundPath = AStar(start, target);
            done = true;

            if (_foundPath != null)
            {
                _gridGenerator.finalPath = _foundPath;
            }
        }

        private List<Node> AStar(Node start, Node target)
        {
            List<Node> path = new List<Node>();

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Node current = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < current.fCost ||
                        (openSet[i].fCost == current.fCost &&
                         openSet[i].hCost < current.hCost))
                    {
                        if (!current.Equals(openSet[i]))
                        {
                            current = openSet[i];
                        }
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current.Equals(target))
                {
                    return Backtrace(start, current);
                }

                foreach (Node neighbour in _gridGenerator.GetNeighbours(current))
                {
                    if (neighbour.obstructed || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    float moveCost = current.gCost + GetDistance(current, neighbour);
                    if (moveCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = moveCost;
                        neighbour.hCost = GetDistance(neighbour, target);
                        neighbour.parent = current;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }


                }
            }

            return null;
        }

        private List<Node> Backtrace(Node start, Node target)
        {
            List<Node> path = new List<Node>();
            Node current = target;

            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }
        

        private int GetDistance(Node a, Node b)
        {
            return ManhattanDistance(a, b);
        }

        private int ManhattanDistance(Node a, Node b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);

            return dx + dy;
        }
        
    }
}
