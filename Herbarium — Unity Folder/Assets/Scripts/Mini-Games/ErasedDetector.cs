using System.Collections.Generic;
using UnityEngine;

public class ErasedDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "ToErase";

    private readonly HashSet<Transform> targets = new();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("ErasedDetector : Parent manquant.");
            return;
        }

        RefreshTargets();

        if (targets.Count == 0)
            Debug.LogWarning($"Aucun objet taggé {targetTag} trouvé.");
    }

    void Update()
    {
        if (validated) return;
        CheckErased();
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

    void CheckErased()
    {
        targets.RemoveWhere(t => t == null || !t.gameObject.activeInHierarchy);

        if (targets.Count > 0)
            return;

        validated = true;
        OnAllErased();
    }

    void OnAllErased()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllErased();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}