using UnityEngine;
using System.Collections.Generic;
using System;

public class MaskSpriteTrail2D : MonoBehaviour
{
    [Header("Mask Shape")]
    public Sprite maskSprite;
    public Vector2 maskScale = Vector2.one;

    [Header("Trail Settings")]
    public float lifetime = 0.5f;
    public float stepDistance = 0.05f;

    [Header("Reveal Target")]
    public SpriteRenderer targetSprite;
    [Range(5, 300)] public int sampleResolution = 20;

    [Header("Debug")]
    public bool showDebug = true;
    public Color debugColor = new Color(0f, 1f, 1f, 0.5f);
    public float debugRadius = 0.05f;
    
    [Header("Reveal Tracking")]
    [Range(0f, 100f)] public float fullyRevealedThreshold = 100f; // <- nouveau champ
    
    [Header("Reveal Tracking")]
    [Range(0, 100)] public int[] revealThresholds = new int[] { 25, 50, 75, 100 };
    private HashSet<int> triggeredThresholds = new HashSet<int>();

    [Space(20)]
    public bool FullyRevealed = false;
    public float RevealPercent = 0f;
    public Action<int> OnRevealThresholdReached;

    Vector3 lastPos;

    class MaskInstance
    {
        public Vector3 position;
        public float time;
        public float startTime;
    }

    readonly List<MaskInstance> masks = new();

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        Vector3 currentPos = transform.position;
        float distance = Vector3.Distance(lastPos, currentPos);

        // --- Spawn masks along distance ---
        if (distance > 0f)
        {
            Vector3 dir = (currentPos - lastPos).normalized;
            float traveled = 0f;

            while (traveled < distance)
            {
                Vector3 pos = lastPos + dir * traveled;
                SpawnMask(pos);
                traveled += stepDistance;
            }

            lastPos = currentPos;
        }

        // --- Update masks lifetime ---
        UpdateMasks();

        // --- Update reveal info ---
        if (targetSprite != null)
        {
            UpdateReveal();
        }
    }

    void SpawnMask(Vector3 position)
    {
        var go = new GameObject("TrailMask");
        go.transform.position = position;
        go.transform.localScale = maskScale;

        var mask = go.AddComponent<SpriteMask>();
        mask.sprite = maskSprite;

        masks.Add(new MaskInstance
        {
            position = position,
            time = lifetime,
            startTime = lifetime
        });

        Destroy(go, lifetime);
    }

    void UpdateMasks()
    {
        for (int i = masks.Count - 1; i >= 0; i--)
        {
            masks[i].time -= Time.deltaTime;
            if (masks[i].time <= 0f)
                masks.RemoveAt(i);
        }
    }

    void UpdateReveal()
    {
        if (targetSprite.sprite == null || masks.Count == 0)
        {
            RevealPercent = 0f;
            FullyRevealed = false;
            return;
        }

        Bounds b = targetSprite.bounds;
        int revealedCount = 0;
        int total = sampleResolution * sampleResolution;

        for (int x = 0; x < sampleResolution; x++)
        {
            for (int y = 0; y < sampleResolution; y++)
            {
                Vector3 samplePoint = new Vector3(
                    Mathf.Lerp(b.min.x, b.max.x, (x + 0.5f) / sampleResolution),
                    Mathf.Lerp(b.min.y, b.max.y, (y + 0.5f) / sampleResolution),
                    b.center.z
                );

                if (IsPointCovered(samplePoint))
                    revealedCount++;
            }
        }

        RevealPercent = 100f * revealedCount / total;
        FullyRevealed = RevealPercent >= fullyRevealedThreshold;


        // Trigger thresholds
        foreach (int threshold in revealThresholds)
        {
            if (!triggeredThresholds.Contains(threshold) && RevealPercent >= threshold)
            {
                triggeredThresholds.Add(threshold);
                OnRevealThresholdReached?.Invoke(threshold);
            }
        }
    }

    bool IsPointCovered(Vector3 point)
    {
        float radius = maskScale.x * 0.5f;

        foreach (var m in masks)
        {
            if (Vector2.Distance(point, m.position) <= radius)
                return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (!showDebug) return;

        // Debug masks
        foreach (var m in masks)
        {
            float t = m.time / m.startTime;
            Gizmos.color = new Color(debugColor.r, debugColor.g, debugColor.b, t);
            Gizmos.DrawSphere(m.position, debugRadius);
        }

        // Debug points of sampling
        if (targetSprite != null && targetSprite.sprite != null)
        {
            Bounds b = targetSprite.bounds;
            float radius = maskScale.x * 0.5f;
            Gizmos.color = FullyRevealed ? Color.green : Color.red;

            for (int x = 0; x < sampleResolution; x++)
            {
                for (int y = 0; y < sampleResolution; y++)
                {
                    Vector3 samplePoint = new Vector3(
                        Mathf.Lerp(b.min.x, b.max.x, (x + 0.5f) / sampleResolution),
                        Mathf.Lerp(b.min.y, b.max.y, (y + 0.5f) / sampleResolution),
                        b.center.z
                    );

                    Gizmos.DrawSphere(samplePoint, radius * 0.3f);
                }
            }
        }
    }
}
