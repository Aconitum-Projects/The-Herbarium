using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ColorDetector : MonoBehaviour
{
    [Header("State")]
    public bool validated = false;

    [Header("Settings")]
    public string colorTag = "Color01";
    public GameObject elementsParent;

    [Header("Animation")]
    public Vector3 offscreenOffset = new Vector3(0, 5f, 0);
    public float tweenDuration = 0.5f;
    public Ease tweenEase = Ease.InQuad;

    private List<Collider2D> targetColliders = new List<Collider2D>();
    private HashSet<Collider2D> colliding = new HashSet<Collider2D>();
    private Vector3 initialPosition;

    private bool alreadyValidated = false;
    private bool waitingForRelease = false;

    void Start()
    {
        initialPosition = transform.position;

        //transform.position = initialPosition + offscreenOffset;
        //transform.DOMove(initialPosition, tweenDuration).SetEase(tweenEase);

        if (elementsParent == null)
        {
            Debug.LogWarning("Ce checker n’a pas de parent, impossible de lister les objets taggés.");
            return;
        }

        foreach (Transform child in elementsParent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(colorTag))
            {
                Collider2D col = child.GetComponent<Collider2D>();
                if (col != null)
                    targetColliders.Add(col);
            }
        }

        if (targetColliders.Count == 0)
            Debug.LogWarning($"Aucun objet avec le tag '{colorTag}' trouvé dans le parent.");
    }

    private void Update()
    {
        colliding.Clear();
        foreach (var target in targetColliders)
        {
            if (GetComponent<Collider2D>().bounds.Intersects(target.bounds))
                colliding.Add(target);
        }

        bool wasValidated = validated;
        validated = colliding.Count == targetColliders.Count;

        if (validated && !alreadyValidated)
        {
            alreadyValidated = true;
            waitingForRelease = true;
        }

        if (waitingForRelease && Input.GetMouseButtonUp(0))
        {
            if (colliding.Count == targetColliders.Count)
            {
                waitingForRelease = false;
                DoValidatedTween();
            }
            else
            {
                waitingForRelease = false;
                alreadyValidated = false;
            }
        }
    }

    private void DoValidatedTween()
    {
        transform.DOMove(initialPosition + offscreenOffset, tweenDuration).SetEase(tweenEase);
    }
    
}