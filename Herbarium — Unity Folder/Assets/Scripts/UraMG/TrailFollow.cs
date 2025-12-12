using System.Collections.Generic;
using UnityEngine;

public class TrailFollow : MonoBehaviour
{
    public LineRenderer line;
    public int maxPoints = 20;
    public float followSpeed = 10f;
    public float trailLength = 5f; // longueur maximale du trail

    private List<Vector3> points = new List<Vector3>();

    void Start()
    {
        Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10f));

        for (int i = 0; i < maxPoints; i++)
            points.Add(startPos);

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePos);

        // Déplacement lissé du dernier point
        Vector3 lastPoint = points[points.Count - 1];
        lastPoint = Vector3.MoveTowards(lastPoint, target, followSpeed * Time.deltaTime);

        points.Add(lastPoint);

        // Ajuster le trail pour garder une longueur constante
        float currentLength = 0f;
        for (int i = points.Count - 1; i > 0; i--)
        {
            currentLength += Vector3.Distance(points[i], points[i - 1]);
            if (currentLength > trailLength)
            {
                points.RemoveRange(0, i - 1);
                break;
            }
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}