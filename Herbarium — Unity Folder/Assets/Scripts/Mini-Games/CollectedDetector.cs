using System.Collections.Generic;
using UnityEngine;

public class CollectedDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "ToCollect";

    private readonly HashSet<Transform> targets = new();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("CollectedDetector : Parent manquant.");
            return;
        }

        RefreshTargets();

        if (targets.Count == 0)
            Debug.LogWarning($"Aucun objet taggé {targetTag} trouvé.");
    }

    void Update()
    {
        if (validated) return;
        CheckCollected();
    }

    void RefreshTargets()
    {
        targets.Clear();

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(targetTag))
                targets.Add(child);
        }
    }

    void CheckCollected()
    {
        if (targets.Count == 0)
            return;

        targets.RemoveWhere(t => t == null);

        foreach (var t in targets)
        {
            var sc = t.GetComponent<SpriteController>();
            if (sc == null || !sc.collected)
                return;
        }

        validated = true;
        OnAllCollected();
    }

    void OnAllCollected()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllCollected();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}