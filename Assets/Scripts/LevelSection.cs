using UnityEngine;
using System.Linq;

public class LevelSection : MonoBehaviour
{
    [System.Serializable]
    public class EndPoint
    {
        public EdgeCollider2D Edge;
        public bool End;

        public Vector2 Point => Edge.transform.TransformPoint(End ? Edge.points[Edge.pointCount - 1] : Edge.points[0]);
    }
    [System.Serializable]
    public class Exit
    {
        public EndPoint Top;
        public EndPoint Bot;
        public bool FlipDirection;

        public bool Valid => Top.Edge != null && Bot.Edge != null;
        public Vector2 Center => (Top.Point + Bot.Point) / 2;
        public Vector2 Direction => Vector2.Perpendicular((Top.Point - Bot.Point).normalized) * (FlipDirection ? -1 : 1);
    }

    public EdgeCollider2D[] Bounds;
    public Exit[] Exits;

    public void OnDrawGizmos()
    {
        // Draw Edges
        foreach(EdgeCollider2D edge in Bounds)
        {
            for(int i = 0; i < edge.pointCount - 1; i++)
            {
                Gizmos.color = Color.blue;
                Vector2 p0 = edge.transform.TransformPoint(edge.points[i]);
                Vector2 p1 = edge.transform.TransformPoint(edge.points[i + 1]);
                Gizmos.DrawLine(p0, p1);
            }
        }

        // Draw Exits
        foreach(Exit exit in Exits)
        {
            if (exit.Valid)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(exit.Top.Point, exit.Bot.Point);
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(exit.Center, exit.Direction * 3);
            }
        }
    }

}
