using UnityEngine;

public class AnimateOnPoints : MonoBehaviour
{
    [Header("Source Controller")]
    public SpriteController source;

    [Header("Animation")]
    public Sprite[] sprites;
    public SpriteRenderer targetRenderer;

    [Header("Easing")]
    [Range(0.1f, 3f)]
    public float easePower = 1f;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (source == null ||
            !source.useFollowPoints ||
            source.followPoints == null ||
            source.followPoints.Count == 0 ||
            source.pointPasses == null ||
            sprites == null ||
            sprites.Length == 0 ||
            targetRenderer == null)
            return;

        // --- calcul du progress global ---
        float progress = 0f;

        for (int i = 0; i < source.pointPasses.Length; i++)
        {
            float t = Mathf.Clamp01(
                (float)source.pointPasses[i] / source.passesPerPoint
            );
            progress += t;
        }

        progress /= source.pointPasses.Length;

        // easing optionnel
        progress = Mathf.Pow(progress, easePower);

        // --- choix de la frame ---
        int frame = Mathf.RoundToInt(
            Mathf.Lerp(0, sprites.Length - 1, progress)
        );

        frame = Mathf.Clamp(frame, 0, sprites.Length - 1);

        targetRenderer.sprite = sprites[frame];
    }
}