using UnityEngine;
using DG.Tweening;

public class SpriteController : MonoBehaviour
{
    public enum Mode
    {
        None,
        Dragable,
        FollowMouse,
        Detachable
    }

    [Header("Mode")] public Mode currentMode = Mode.Dragable;

    [Header("Follow Mouse Settings")]
    public bool followX = true;
    public bool followY = true;
    public float followSpeed = 10f;

    [Header("Detachable Settings")]
    public Vector2 maxScale = new Vector2(2f, 2f);
    public bool detachToDragable = true;
    public bool detachToNone = false;
    public float distanceMultiplier = 1f;
    public float scaleDuration = 0.2f;

    private Vector3 initialScale;
    private Vector3 detachOrigin;

    private Camera mainCam;
    private Vector3 offset;
    private bool isDragging = false;

    void Awake()
    {
        mainCam = Camera.main;
        initialScale = transform.localScale;
    }

    void Update()
    {
        switch (currentMode)
        {
            case Mode.Dragable: break;
            case Mode.FollowMouse: FollowMouseUpdate(); break;
            case Mode.Detachable: if (isDragging) DetachableUpdate(); break;
        }
    }

    // ------------------ Dragable ------------------
    void OnMouseDown()
    {
        if (currentMode == Mode.Dragable || currentMode == Mode.Detachable)
        {
            isDragging = true;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

            offset = transform.position - worldPos;

            if (currentMode == Mode.Detachable)
            {
                detachOrigin = transform.position;
            }
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        if (currentMode == Mode.Dragable)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

            transform.position = worldPos + offset;
        }
        else if (currentMode == Mode.Detachable)
        {
            DetachableUpdate();
        }
    }

    void OnMouseUp()
    {
        if (currentMode == Mode.Detachable && isDragging)
        {
            float distance = Vector3.Distance(transform.position, detachOrigin);
            float t = Mathf.Clamp01(distance * distanceMultiplier / 1f);

            if (t < 1f)
            {
                transform.DOMove(detachOrigin, 0.3f).SetEase(Ease.OutBack);
                transform.DOScale(initialScale, 0.3f).SetEase(Ease.OutBack);
            }
        }

        isDragging = false;
    }


    // ------------------ Follow Mouse ------------------
    private void FollowMouseUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        Vector3 targetPos = transform.position;
        if (followX) targetPos.x = worldPos.x;
        if (followY) targetPos.y = worldPos.y;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    // ------------------ Detachable ------------------
    private void DetachableUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        float distance = Vector3.Distance(worldPos, detachOrigin);
        float t = Mathf.Clamp01(distance * distanceMultiplier / 1f); // facteur normalisé 0-1

        // Calcul du scale cible en Vector2
        Vector2 targetScale = new Vector2(
            Mathf.Lerp(initialScale.x, maxScale.x, t),
            Mathf.Lerp(initialScale.y, maxScale.y, t)
        );

        // Tween pour smooth le scale
        transform.DOScale(new Vector3(targetScale.x, targetScale.y, transform.localScale.z), scaleDuration);

        if (t >= 1f)
        {
            isDragging = false;

            // Remettre le scale initial avec un tween rapide
            transform.DOScale(initialScale, 0.1f);

            // Prendre la position de la souris
            transform.position = worldPos;

            if (detachToDragable) currentMode = Mode.Dragable;
            else if (detachToNone) currentMode = Mode.None;
        }
    }
}