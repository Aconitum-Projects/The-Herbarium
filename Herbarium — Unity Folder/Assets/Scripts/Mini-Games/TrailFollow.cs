using System.Collections.Generic;
using UnityEngine;

public class TrailFollow : MonoBehaviour
{
    [Header("Trail")]
    public LineRenderer trailLine;
    public int maxTrailPoints = 100;
    public float followSpeed = 10f;
    
    [Header("Outline")]
    public LineRenderer outlineLine;

    [Header("Shape")]
    public LineRenderer shapeLine;       // forme cible
    public float maxDistance = 0.3f;    // tolérance
    public float successThreshold = 0.8f; // 80% pour réussir
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Oval Settings")]
    public Vector2 ovalCenter = Vector2.zero;
    public float radiusX = 3f;
    public float radiusY = 2f;
    public int ovalPointsCount = 50;

    private List<Vector3> trailPoints = new();
    private Vector3[] shapePoints;
    private bool shapeCompleted = false;

    void Start()
    {
        // Générer automatiquement l'ovale
        shapePoints = new Vector3[ovalPointsCount];
        for (int i = 0; i < ovalPointsCount; i++)
        {
            float angle = 2f * Mathf.PI * i / ovalPointsCount;
            float x = ovalCenter.x + Mathf.Cos(angle) * radiusX;
            float y = ovalCenter.y + Mathf.Sin(angle) * radiusY;
            shapePoints[i] = new Vector3(x, y, 0f);
        }

        // Appliquer les points au LineRenderer de la forme
        shapeLine.positionCount = shapePoints.Length;
        shapeLine.SetPositions(shapePoints);

        // Initialise le trail au centre
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

        // Récupération souris
        Vector3 mouse = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)
        );

        // Lerp pour trail fluide
        Vector3 newPoint = Vector3.Lerp(trailPoints[^1], mouse, followSpeed * Time.deltaTime);
        trailPoints.Add(newPoint);

        if (trailPoints.Count > maxTrailPoints)
            trailPoints.RemoveAt(0);

        trailLine.positionCount = trailPoints.Count;
        trailLine.SetPositions(trailPoints.ToArray());
        outlineLine.positionCount = trailPoints.Count;
        outlineLine.SetPositions(trailPoints.ToArray());

        // Vérifier la forme
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

        // Feedback visuel
        shapeLine.startColor = shapeLine.endColor = ratio >= successThreshold ? correctColor : wrongColor;

        if (ratio >= successThreshold && !shapeCompleted)
        {
            shapeCompleted = true;
            Debug.Log("Forme réussie !");
        }
    }
}
