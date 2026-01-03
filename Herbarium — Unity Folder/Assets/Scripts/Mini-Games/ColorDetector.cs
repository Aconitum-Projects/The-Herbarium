using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColorDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public string targetTag = "Color01";
    public Transform elementsParent;

    [Header("Animation")]
    public Vector3 offscreenOffset = new Vector3(0, 5f, 0);
    public float tweenDuration = 0.5f;
    public Ease tweenEase = Ease.InQuad;

    private List<Collider2D> targets = new List<Collider2D>();
    private HashSet<Collider2D> colliding = new HashSet<Collider2D>();

    private Vector3 initialPos;

    private bool waitingForRelease = false;
    private bool readyToValidate = false;

    void Start()
    {
        initialPos = transform.position;

        if (elementsParent == null)
        {
            Debug.LogWarning("ColorDetector : aucun parent défini.");
            return;
        }

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(targetTag))
            {
                var col = child.GetComponent<Collider2D>();
                if (col != null)
                    targets.Add(col);
            }
        }

        if (targets.Count == 0)
            Debug.LogWarning($"ColorDetector : aucun objet taggé {targetTag}.");
    }

    void Update()
    {
        if (validated) return;

        colliding.Clear();
        foreach (var t in targets)
        {
            if (t != null && GetComponent<Collider2D>().bounds.Intersects(t.bounds))
                colliding.Add(t);
        }

        bool allMatch = colliding.Count == targets.Count;

        if (allMatch && !readyToValidate)
        {
            readyToValidate = true;
            waitingForRelease = true;
        }

        if (waitingForRelease && Input.GetMouseButtonUp(0))
        {
            waitingForRelease = false;

            if (colliding.Count == targets.Count)
            {
                DoValidatedTween();
            }
            else
            {
                readyToValidate = false;
            }
        }
    }

    void DoValidatedTween()
    {
        transform.DOMove(initialPos + offscreenOffset, tweenDuration)
            .SetEase(tweenEase)
            .OnComplete(() =>
            {
                validated = true;
                NotifyManager();
            });
    }

    void NotifyManager()
    {
        var vc = transform.parent.GetComponentInParent<VictoryChecker>();
        if (vc != null)
            vc.CheckMatchingColors();
    }
    
}
