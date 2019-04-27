using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinding
{
    
    /// <summary>
    /// Represents a node in a regular grid of nodes used in pathfinding.
    /// </summary>

    public class Node
    {

        public int x;
        public int y;

        public bool obstructed;
        public Vector3 position;

        public Node parent;
        public float gCost; /* The accumulated cost so far. */
        public float hCost; /* The distance to the goal from this node. */

        public float fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public Node(bool isObstructed, Vector3 worldPosition, int gridX, int gridY)
        {
            obstructed = isObstructed;
            position = worldPosition;
            x = gridX;
            y = gridY;
        }

    }
}