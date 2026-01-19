using UnityEngine;

public class TouchedDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;
    public int touchCount = 0;

    [Header("Settings")]
    public string toucherTag = "Toucher";
    public int requiredTouches = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (validated) return;
        if (!other.CompareTag(toucherTag)) return;

        touchCount++;

        if (touchCount >= requiredTouches)
        {
            validated = true;
            OnTouchedValidated();
        }
    }

    void OnTouchedValidated()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckAllTouched();
        else
            Debug.Log("VictoryCheckerNotFound");
    }
}