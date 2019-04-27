using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class GridGenerator : MonoBehaviour
    {

        public LayerMask obstaclesMask;
        public Vector2 gridSize;
        public float nodeRadius;
        public float nodeSpacing; /* Visual purpose only */

        public Vector3 startPosition;
        public Vector3 endPosition;
        
        public List<Node> finalPath;

        private Node[,] nodes;
        private float nodeDiameter;
        private int sizeX;
        private int sizeY;


        private void Start()
        {
            nodeDiameter = nodeRadius * 2;
            sizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
            sizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
            Generate();
        }

        private void Generate()
        {
            nodes = new Node[sizeX, sizeY];

            Vector3 bottomLeft = transform.position - 
                                 Vector3.right * gridSize.x * 0.5f - 
                                 Vector3.up * gridSize.y * 0.5f;
            
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Vector3 worldPoint3 = bottomLeft + 
                        Vector3.right * (x * nodeDiameter + nodeRadius) + 
                        Vector3.up * (y * nodeDiameter + nodeRadius);
                    
                    Vector2 worldPoint2 = new Vector2(worldPoint3.x, worldPoint3.y);

                    bool obstructed = (Physics2D.OverlapCircle(worldPoint2, nodeRadius, obstaclesMask) != null);
                    
                    nodes[x, y] = new Node(obstructed, worldPoint3, x, y);
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
                    if ((node.x + i) >= 0 && (node.x + i) < sizeX &&
                        (node.y + j) >= 0 && (node.y + j) < sizeY)
                    {
                        neighbours.Add(nodes[node.x + i, node.y + j]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPosition(Vector3 worldPosition)
        {
            
            float fx = Mathf.Clamp01((worldPosition.x + gridSize.x * 0.5f) / gridSize.x);
            float fy = Mathf.Clamp01((worldPosition.y + gridSize.y * 0.5f) / gridSize.y);

            int ix = Mathf.RoundToInt((sizeX - 1) * fx);
            int iy = Mathf.RoundToInt((sizeY - 1) * fy);

            ix = (ix < 0) ? 0 : ix;
            ix = (ix >= sizeX) ? (sizeX - 1) : ix;
            iy = (iy < 0) ? 0 : iy;
            iy = (iy >= sizeY) ? (sizeY - 1) : iy;
            
            return nodes[ix, iy];
        }


        private void OnDrawGizmos()
        {

            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 1));

            if (nodes != null)
            {
                foreach (Node n in nodes)
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

                    Utilities.drawRectangle(n.position, Vector2.one * (nodeDiameter - nodeSpacing));
                }
            }
        }
        
        // Singleton
        public static GridGenerator instance;
        public static GridGenerator GetInstance()
        {
            return instance;
        }

        void Awake()
        {
            instance = this;
        }
    }

}