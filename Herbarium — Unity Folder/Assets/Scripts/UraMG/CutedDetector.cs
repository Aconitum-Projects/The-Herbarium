using System.Collections.Generic;
using UnityEngine;

public class CutedDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "ToCut";

    private List<SpriteController> targets = new List<SpriteController>();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("CutedDetector : Aucun parent assigné.");
            return;
        }

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(targetTag))
            {
                SpriteController sc = child.GetComponent<SpriteController>();
                if (sc != null)
                {
                    targets.Add(sc);
                }
                else
                {
                    Debug.LogWarning($"L'objet '{child.name}' a le tag {targetTag} mais n'a pas de SpriteController.");
                }
            }
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
            if (t == null || !t.isCut)
                return;
        }

        validated = true;
        OnAllCuted();
    }

    void OnAllCuted()
    {
        var vc = transform.parent.GetComponent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllCuted();
    }
}