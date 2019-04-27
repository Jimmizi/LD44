using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    
    /// <summary>
    /// Utilities and helpers for pathfinding.
    /// </summary>

    public static class Utilities
    {
        public static void drawRectangle(Vector3 center, Vector2 dimensions)
        {
            Vector3 bottomLeft = center - 
                                 Vector3.right * dimensions.x * 0.5f - 
                                 Vector3.up * dimensions.y * 0.5f;
            Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.up * dimensions.y);
            Gizmos.DrawLine(bottomLeft + Vector3.up * dimensions.y,
                bottomLeft + Vector3.right * dimensions.x + Vector3.up * dimensions.y);
            Gizmos.DrawLine(bottomLeft + Vector3.right * dimensions.x + Vector3.up * dimensions.y,
                bottomLeft + Vector3.right * dimensions.x);
            Gizmos.DrawLine(bottomLeft + Vector3.right * dimensions.x, bottomLeft);
        }
    }
}
