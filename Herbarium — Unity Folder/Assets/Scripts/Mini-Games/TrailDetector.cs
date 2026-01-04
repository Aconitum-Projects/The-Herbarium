using UnityEngine;

public class TrailDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public TrailFollow trailFollow;

    void Start()
    {
        if (trailFollow == null)
        {
            trailFollow = GetComponentInChildren<TrailFollow>();
        }

        if (trailFollow == null)
        {
            Debug.LogWarning("TrailDetector : Aucun TrailFollow trouvé.");
        }
    }

    void Update()
    {
        if (validated) return;
        if (trailFollow == null) return;

        if (!trailFollow.isValid) return;

        validated = true;
        OnTrailValidated();
    }

    void OnTrailValidated()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckTrailValidated();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}