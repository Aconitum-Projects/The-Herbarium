using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class BoneHanging2D : MonoBehaviour
{
    [Header("Reference (pseudo-parent)")]
    public Transform referenceParent;

    [Header("Gravity / Down")]
    public Vector2 worldDown = Vector2.down;

    [Header("Wind")]
    public float windAmplitude = 5f;
    public float windSpeed = 1f;
    public float windRandomness = 0.5f;

    [Header("Damping")]
    public float smooth = 6f;

    float noiseOffset;
    double lastEditorTime;

    void OnEnable()
    {
        noiseOffset = Random.Range(0f, 1000f);

#if UNITY_EDITOR
        lastEditorTime = EditorApplication.timeSinceStartup;
#endif
    }

    void LateUpdate()
    {
        if (referenceParent == null) return;

        CopyParentPlacement();
        ApplyWorldDownRotation();
    }

    void CopyParentPlacement()
    {
        // Copie position monde (pas de parentage réel)
        transform.position = referenceParent.position;

        // Optionnel : copier l’échelle si besoin
        // transform.localScale = referenceParent.lossyScale;
    }

    void ApplyWorldDownRotation()
    {
        float deltaTime;

        if (Application.isPlaying)
        {
            deltaTime = Time.deltaTime;
        }
        else
        {
#if UNITY_EDITOR
            double now = EditorApplication.timeSinceStartup;
            deltaTime = (float)(now - lastEditorTime);
            lastEditorTime = now;
#else
            deltaTime = 0f;
#endif
        }

        if (deltaTime <= 0f) return;

        float baseAngle = Mathf.Atan2(worldDown.y, worldDown.x) * Mathf.Rad2Deg - 90f;
        Quaternion worldDownRot = Quaternion.Euler(0f, 0f, baseAngle);

        float time = Application.isPlaying
            ? Time.time
            : (float)EditorApplication.timeSinceStartup;

        float noise = Mathf.PerlinNoise(time * windSpeed + noiseOffset, 0f);
        float windAngle = (noise - 0.5f) * 2f * windAmplitude * windRandomness;

        Quaternion targetWorldRot =
            worldDownRot * Quaternion.Euler(0f, 0f, windAngle);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetWorldRot,
            deltaTime * smooth
        );
    }
}
