using System.Collections.Generic;
using UnityEngine;

public class DetachDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public Transform elementsParent;
    public string targetTag = "ToDetach";

    private List<SpriteController> targets = new List<SpriteController>();

    void Start()
    {
        if (elementsParent == null)
        {
            Debug.LogWarning("DetachDetector : Aucun parent assigné.");
            return;
        }

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(targetTag))
            {
                SpriteController d = child.GetComponent<SpriteController>();
                if (d != null)
                {
                    targets.Add(d);
                }
                else
                {
                    Debug.LogWarning($"L'objet '{child.name}' a le tag {targetTag} mais n'a pas de script Detachable.");
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
            if (t == null || !t.isDetached)
                return;
        }

        validated = true;
        OnAllDetached();
    }
    
    

    void OnAllDetached()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllDetached();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}