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

    [Header("Outline")]
    public LineRenderer outlineLine;

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

        Vector3 startPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 10f)
        );

        for (int i = 0; i < maxTrailPoints; i++)
            trailPoints.Add(startPos);

        trailLine.positionCount = trailPoints.Count;
        trailLine.SetPositions(trailPoints.ToArray());
        outlineLine.positionCount = trailPoints.Count;
        outlineLine.SetPositions(trailPoints.ToArray());
        
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

        outlineLine.positionCount = trailPoints.Count;
        outlineLine.SetPositions(trailPoints.ToArray());

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
}
