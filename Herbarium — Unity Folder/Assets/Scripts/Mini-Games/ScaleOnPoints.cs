using UnityEngine;

public class ScaleOnPoints : MonoBehaviour
{
    [Header("Source Controller")]
    public SpriteController source;
    public Vector3 minScale = Vector3.one * 0.5f;
    public Vector3 maxScale = Vector3.one;

    void Update()
    {
        if (source == null || !source.useFollowPoints || source.followPoints == null || source.followPoints.Count == 0)
            return;

        // Calcul du pourcentage de points validés
        float progress = 0f;
        for (int i = 0; i < source.pointPasses.Length; i++)
        {
            float t = Mathf.Clamp01((float)source.pointPasses[i] / source.passesPerPoint);
            progress += t;
        }
        progress /= source.pointPasses.Length;

        // Lerp entre min et max scale avec easing
        transform.localScale = Vector3.Lerp(minScale, maxScale, Mathf.Pow(progress, 0.5f));
    }
}