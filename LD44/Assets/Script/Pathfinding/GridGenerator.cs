using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Pathfinding
{
    
    /// <summary>
    /// Generates grid to apply pathfinding on.
    /// </summary>

    public class GridGenerator : MonoBehaviour
    {

        public LayerMask obstaclesMask;
        public Vector2 gridSize;
        public float nodeRadius;
        public float nodeSpacing; /* Visual purpose only */

        public Vector3 startPosition;
        public Vector3 endPosition;
        
        public List<Node> finalPath;

        private Node[,] _nodes;
        private float _nodeDiameter;
        private int _sizeX;
        private int _sizeY;


        private void Start()
        {
            _nodeDiameter = nodeRadius * 2;
            _sizeX = Mathf.RoundToInt(gridSize.x / _nodeDiameter);
            _sizeY = Mathf.RoundToInt(gridSize.y / _nodeDiameter);
            Generate();
        }

        private void Generate()
        {
            _nodes = new Node[_sizeX, _sizeY];

            Vector3 bottomLeft = transform.position - 
                                 Vector3.right * gridSize.x * 0.5f - 
                                 Vector3.up * gridSize.y * 0.5f;
            
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    Vector3 worldPoint3 = bottomLeft + 
                        Vector3.right * (x * _nodeDiameter + nodeRadius) + 
                        Vector3.up * (y * _nodeDiameter + nodeRadius);
                    
                    Vector2 worldPoint2 = new Vector2(worldPoint3.x, worldPoint3.y);

                    
                    bool obstructed = (Physics2D.OverlapCircle(worldPoint2, nodeRadius, obstaclesMask) != null);
                    
                    _nodes[x, y] = new Node(obstructed, worldPoint3, x, y);
                }
            }
            
        }

        public void DetectObstacles()
        {
            
            Vector3 bottomLeft = transform.position - 
                                 Vector3.right * gridSize.x * 0.5f - 
                                 Vector3.up * gridSize.y * 0.5f;
            
            for (int x = 0; x < _sizeX; x++)
            {
                for (int y = 0; y < _sizeY; y++)
                {
                    Vector3 worldPoint3 = bottomLeft +
                                          Vector3.right * (x * _nodeDiameter + nodeRadius) +
                                          Vector3.up * (y * _nodeDiameter + nodeRadius);

                    Vector2 worldPoint2 = new Vector2(worldPoint3.x, worldPoint3.y);
                    
                    _nodes[x, y].obstructed = (Physics2D.OverlapCircle(worldPoint2, nodeRadius, obstaclesMask) != null);
                }
            }

        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((node.x + i) >= 0 && (node.x + i) < _sizeX &&
                        (node.y + j) >= 0 && (node.y + j) < _sizeY)
                    {
                        neighbours.Add(_nodes[node.x + i, node.y + j]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPosition(Vector3 worldPosition)
        {
            
            float fx = Mathf.Clamp01((worldPosition.x + gridSize.x * 0.5f) / gridSize.x);
            float fy = Mathf.Clamp01((worldPosition.y + gridSize.y * 0.5f) / gridSize.y);

            int ix = Mathf.RoundToInt((_sizeX - 1) * fx);
            int iy = Mathf.RoundToInt((_sizeY - 1) * fy);

            ix = (ix < 0) ? 0 : ix;
            ix = (ix >= _sizeX) ? (_sizeX - 1) : ix;
            iy = (iy < 0) ? 0 : iy;
            iy = (iy >= _sizeY) ? (_sizeY - 1) : iy;
            
            return _nodes[ix, iy];
        }


        private void OnDrawGizmos()
        {

            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

            if (_nodes != null)
            {
                foreach (Node n in _nodes)
                {
                    if (n.obstructed)
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.grey;
                    }


                    if (finalPath != null)
                    {
                        if (finalPath.Contains(n))
                        {
                            Gizmos.color = Color.magenta;
                        }

                    }

                    if (NodeFromWorldPosition(startPosition).Equals(n) ||
                        NodeFromWorldPosition(endPosition).Equals(n))
                    {
                        Gizmos.color = Color.red;
                    }

                    Utilities.drawRectangle(n.position, Vector2.one * (_nodeDiameter - nodeSpacing));
                }
            }
        }
        
        // Singleton
        private static GridGenerator _instance;
        
        public static GridGenerator GetInstance()
        {
            return _instance;
        }

        void Awake()
        {
            _instance = this;
        }
    }

}