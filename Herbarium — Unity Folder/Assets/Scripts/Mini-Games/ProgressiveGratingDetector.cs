using System.Collections.Generic;
using UnityEngine;

public class ProgressiveGratingDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "ToFollow";

    private readonly List<SpriteController> targets = new();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("ProgressiveGratingDetector : Parent manquant.");
            return;
        }

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (!child.CompareTag(targetTag)) continue;

            var sc = child.GetComponent<SpriteController>();
            if (sc != null)
                targets.Add(sc);
        }

        if (targets.Count == 0)
            Debug.LogWarning($"Aucun objet taggé {targetTag} trouvé.");
    }

    void Update()
    {
        if (validated) return;
        if (targets.Count == 0) return;

        foreach (var t in targets)
        {
            if (t == null || !t.progressiveValidated)
                return;
        }

        validated = true;
        OnValidated();
    }

    void OnValidated()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllProgressed();
    }
}