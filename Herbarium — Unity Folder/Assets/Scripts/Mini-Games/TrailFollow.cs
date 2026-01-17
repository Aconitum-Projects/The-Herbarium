using System.Collections.Generic;
using UnityEngine;

public class TrailFollow : MonoBehaviour
{
    [Header("Trail")]
    public LineRenderer trailLine;
    public int maxTrailPoints = 100;
    public float followSpeed = 10f;
    
    [Header("Vine")]
    public float segmentLength = 0.15f;
    [Range(0f, 1f)]
    public float stiffness = 0.6f;
    
    [Header("Vine Start")]
    public Transform vineStart;
    public Vector2 startDirection = Vector2.right;

    [Header("Shape")]
    public LineRenderer shapeLine;
    public float maxDistance = 0.3f;
    public float successThreshold = 0.8f;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Oval Settings")]
    public Vector2 ovalCenter = Vector2.zero;
    public float radiusX = 3f;
    public float radiusY = 2f;
    public int ovalPointsCount = 50;
    
    [Header("State")]
    public bool isValid = false;

    private List<Vector3> trailPoints = new();
    private Vector3[] shapePoints;
    private bool shapeCompleted = false;

    void Start()
    {
        shapePoints = new Vector3[ovalPointsCount];
        for (int i = 0; i < ovalPointsCount; i++)
        {
            float angle = 2f * Mathf.PI * i / ovalPointsCount;
            float x = ovalCenter.x + Mathf.Cos(angle) * radiusX;
            float y = ovalCenter.y + Mathf.Sin(angle) * radiusY;
            shapePoints[i] = new Vector3(x, y, 0f);
        }

        shapeLine.positionCount = shapePoints.Length;
        shapeLine.SetPositions(shapePoints);

        Vector3 startPos = vineStart != null
            ? vineStart.position
            : transform.position;

        Vector3 dir = startDirection.normalized;
        
        for (int i = 0; i < maxTrailPoints; i++)
        {
            trailPoints.Add(
                startPos + dir * segmentLength * (maxTrailPoints - 1 - i)
            );
        }
        
        trailLine.positionCount = trailPoints.Count;
        trailLine.SetPositions(trailPoints.ToArray());
        
    }

    void Update()
    {
        if (shapeCompleted) return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)
        );

        trailPoints[^1] = Vector3.Lerp(
            trailPoints[^1],
            mouse,
            followSpeed * Time.deltaTime
        );

        for (int i = trailPoints.Count - 2; i >= 0; i--)
        {
            Vector3 targetPos =
                trailPoints[i + 1] +
                (trailPoints[i] - trailPoints[i + 1]).normalized * segmentLength;

            trailPoints[i] = Vector3.Lerp(
                trailPoints[i],
                targetPos,
                stiffness
            );
        }

        trailLine.positionCount = trailPoints.Count;
        trailLine.SetPositions(trailPoints.ToArray());

        CheckShape();
    }

    void CheckShape()
    {
        int correctPoints = 0;

        foreach (var sp in shapePoints)
        {
            foreach (var tp in trailPoints)
            {
                if (Vector3.Distance(sp, tp) <= maxDistance)
                {
                    correctPoints++;
                    break;
                }
            }
        }

        float ratio = (float)correctPoints / shapePoints.Length;

        shapeLine.startColor = shapeLine.endColor = ratio >= successThreshold ? correctColor : wrongColor;

        isValid = ratio >= successThreshold;

        if (isValid && !shapeCompleted)
        {
            shapeCompleted = true;
        }

    }
    
    void OnDrawGizmos()
    {
        if (vineStart == null) return;

        Vector3 startPos = vineStart.position;
        Vector3 dir = ((Vector3)startDirection).normalized;

        // Point de départ
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(startPos, 0.08f);

        // Direction
        Gizmos.DrawLine(
            startPos,
            startPos + dir * segmentLength * 3f
        );

        // Preview de la liane
        Gizmos.color = new Color(0f, 1f, 1f, 0.6f);

        Vector3 prev = startPos;
        for (int i = 1; i < maxTrailPoints; i++)
        {
            Vector3 p = startPos + dir * segmentLength * i;
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= ovalPointsCount; i++)
        {
            float angle = 2f * Mathf.PI * i / ovalPointsCount;
            Vector3 p = new Vector3(
                ovalCenter.x + Mathf.Cos(angle) * radiusX,
                ovalCenter.y + Mathf.Sin(angle) * radiusY,
                0f
            );

            if (i > 0)
                Gizmos.DrawLine(prev, p);

            prev = p;
        }
    }
    
}
