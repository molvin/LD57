using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonDraw : MonoBehaviour
{
    private List<LineRenderer> lineRenderers;
    private PolygonCollider2D polygonCollider;

    void Start()
    {
        polygonCollider= GetComponent<PolygonCollider2D>();

        lineRenderers = new();
        for (int i = 0; i < polygonCollider.pathCount; i++)
        {
            lineRenderers.Add(gameObject.AddComponent<LineRenderer>());
        }

        InitializeLineRenderer();
    }

    private void InitializeLineRenderer()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            List<Vector3> pathPoints = polygonCollider.GetPath(i).Select(p => (Vector3)p + transform.position).ToList();
            pathPoints.Add(pathPoints[0]);
            var lineRenderer = lineRenderers[i];
            lineRenderer.positionCount = pathPoints.Count();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor = Color.cyan;
            lineRenderer.SetPositions(pathPoints.ToArray());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        polygonCollider = GetComponent<PolygonCollider2D>();
        for (int p = 0; p < polygonCollider.pathCount; p++)
        {
            List<Vector3> pathPoints = polygonCollider.GetPath(p).Select(p => (Vector3)p + transform.position).ToList();
            pathPoints.Add(pathPoints[0]);

            for (int i = 1; i < pathPoints.Count; i++)
            {
                Gizmos.DrawLine(pathPoints[i - 1], pathPoints[i]);
            }
        }
    }
}

