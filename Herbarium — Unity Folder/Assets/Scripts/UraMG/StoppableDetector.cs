using System.Collections.Generic;
using UnityEngine;

public class StoppableDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "Stoppable";

    private readonly HashSet<Transform> targets = new();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("StoppableDetector : Parent manquant.");
            return;
        }

        RefreshTargets();

        if (targets.Count == 0)
            Debug.LogWarning($"Aucun objet taggé {targetTag} trouvé.");
    }

    void Update()
    {
        if (validated) return;
        CheckStoppable();
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

    void CheckStoppable()
    {
        targets.RemoveWhere(t => t == null || !t.gameObject.activeInHierarchy);

        foreach (var t in targets)
        {
            var sc = t.GetComponent<SpriteController>();
            if (sc == null && sc.isStopped) return;
        }

        validated = true;
        OnAllStopped();
    }

    void OnAllStopped()
    {
        var vc = transform.parent.GetComponent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllStopped();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}