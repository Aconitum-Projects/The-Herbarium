using System.Collections.Generic;
using UnityEngine;

public class FilledDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "ToFill";

    private readonly HashSet<Transform> targets = new();
    private readonly HashSet<Transform> inside = new();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("FilledDetector : Parent manquant.");
            return;
        }

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(targetTag))
                targets.Add(child);
        }

        if (targets.Count == 0)
            Debug.LogWarning($"Aucun objet taggé {targetTag} trouvé.");
    }

    void Update()
    {
        if (validated) return;
        if (Input.GetMouseButtonUp(0))
        {
            CheckFilled();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (validated) return;
        if (!other.CompareTag(targetTag)) return;

        inside.Add(other.transform);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        inside.Remove(other.transform);
    }

    void CheckFilled()
    {
        foreach (var t in targets)
        {
            if (!inside.Contains(t))
                return;
        }

        validated = true;
        OnAllFilled();
    }

    void OnAllFilled()
    {
        var vc = transform.parent.GetComponent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllFilled();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}