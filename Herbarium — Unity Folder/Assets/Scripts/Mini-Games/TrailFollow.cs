using System.Collections.Generic;
using UnityEngine;

public class TrailFollow : MonoBehaviour
{
    public LineRenderer line;
    public int maxPoints = 20;
    public float followSpeed = 10f;
    public float trailLength = 5f;

    [Header("Spiral ellipse")]
    public Vector2 center;
    public Vector2 radius = new Vector2(4f, 2f);
    public float spiralStrength = 0.15f;
    public Vector2 spiralClamp = new Vector2(0.2f, 1.5f);

    List<Vector3> points = new();

    void Start()
    {
        center = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 10f)
        );

        for (int i = 0; i < maxPoints; i++)
            points.Add(center);

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }

    void Update()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)
        );

        Vector2 dir = mouse - (Vector3)center;
        float angle = Mathf.Atan2(dir.y, dir.x);

        float spiral = Mathf.Clamp(
            Mathf.Abs(angle) * spiralStrength,
            spiralClamp.x,
            spiralClamp.y
        );

        Vector3 target = center + new Vector2(
            Mathf.Cos(angle) * radius.x,
            Mathf.Sin(angle) * radius.y
        ) * spiral;

        points.Add(Vector3.MoveTowards(
            points[^1],
            target,
            followSpeed * Time.deltaTime
        ));

        TrimTrail();

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }

    void TrimTrail()
    {
        float length = 0f;

        for (int i = points.Count - 1; i > 0; i--)
        {
            length += Vector3.Distance(points[i], points[i - 1]);
            if (length > trailLength)
            {
                points.RemoveRange(0, i - 1);
                return;
            }
        }
    }
}