using UnityEngine;

public class ScaleOnShake : MonoBehaviour
{
    [Header("Target Shake Object")]
    public SpriteController shakeSource; // objet avec le script SpriteController
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f); // scale min
    public Vector3 maxScale = Vector3.one; // scale max

    void Update()
    {
        if (shakeSource == null) return;

        // shakeAccum varie entre 0 et 1
        float t = Mathf.Clamp01(shakeSource.shakeAccum);

        // lerp entre maxScale et minScale selon t
        transform.localScale = Vector3.Lerp(maxScale, minScale, Mathf.Pow(t, 0.5f));
    }
}