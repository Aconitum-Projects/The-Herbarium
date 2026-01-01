using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class SpriteController : MonoBehaviour
{
    public enum Mode
    {
        None,
        Draggable,
        FollowMouse,
        Detachable,
        Cuttable,
        Fillable,
        Erasable,
        Stoppable
    }

    public enum DetachType
    {
        None,
        Draggable,
        Fall
    }
    public enum DetachVisualMode
    {
        Scalable,
        Animated
    }

    // Mode
    public Mode currentMode = Mode.None;
    public bool isShakeable = false;

    // Follow Mouse Settings
    public bool followX = true;
    public bool followY = true;
    public float followSpeed = 20f;
    public Collider2D cutterCollider;
    public bool limitX = false;
    public Vector2 xLimits = new Vector2(-5f, 5f);
    public bool limitY = false;
    public Vector2 yLimits = new Vector2(-3f, 3f);

    // Detachable Settings
    public Vector2 maxScale = new Vector2(.05f, 1.5f);
    public float detachThreshold = 5f;
    public float scaleDuration = 0.2f;
    public float fallDistance = 20f;
    public float fallDuration = 0.4f;
    public bool isDetached = false;
    public DetachType detachType = DetachType.None;
    public bool keepDetachedScale = false;
    public DetachVisualMode detachVisualMode = DetachVisualMode.Scalable;

    // Animated Detach
    public Sprite[] animatedDetachSprites;

    // Cuttable Settings
    public Collider2D cuttableCollider;
    public bool isCut = false;

    // Shakeable Settings
    public bool isShaken = false;
    public float shakeThreshold = 15f;
    public float shakeMultiplier = 50f;
    public float shakeResetTime = 0.2f;
    public BoxCollider2D eraserCollider;

    // Fillable Settings
    public bool fillX = true;
    public bool fillY = true;
    public float fillOffsetX = 0f;
    public float fillOffsetY = 0f;
    public float fillDuration = 1.2f;
    public Ease fillEase = Ease.InOutSine;
    
    // Erasable Settings
    public float eraseSpeed = 0.4f;
    public float minAlpha = 0f;
    
    // Stoppable Settings
    public Vector3 stoppableTargetOffset;
    public float stoppableDuration = 1f;
    public Ease stoppableEase = Ease.InOutSine;
    public bool isStopped = false;

    private Vector3 stoppableOrigin;
    private Tween stoppableTween;
    private SpriteRenderer spriteRenderer;
    private bool isBeingErased = false;
    private Vector3 lastPos;
    private float shakeTimer = 0f;
    private Vector3 initialScale;
    private Vector3 detachOrigin;
    private Camera mainCam;
    private Vector3 offset;
    private bool isDragging = false;
    private Tween fillTween;
    private Vector3 fillStartPos;
    void Awake()
    {
        mainCam = Camera.main;
        initialScale = transform.localScale;
        lastPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (currentMode == Mode.Stoppable)
            StartStoppable();
    }

    void Update()
    {
        switch (currentMode)
        {
            case Mode.Draggable: break;
            case Mode.FollowMouse: FollowMouseUpdate(); break;
            case Mode.Detachable:
                if (isDragging) DetachableUpdate();
                break;
            case Mode.Cuttable: CheckCuttable(); break;
            case Mode.Fillable: FillableUpdate(); break;
            case Mode.Erasable:
                ErasableUpdate();
                break;
            case Mode.Stoppable:
                StoppableUpdate();
                break;


        }

        // ----------- Shake Detection -----------
        if (isShakeable)

        {
            float speed = (transform.position - lastPos).magnitude / Time.deltaTime;
            if (!isShaken && speed > shakeThreshold * shakeMultiplier)
            {
                isShaken = true;
                shakeTimer = shakeResetTime;
            }
        }

        if (isShaken)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
                isShaken = false;
        }

        lastPos = transform.position;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentMode != Mode.Erasable) return;
        if (eraserCollider == null) return;

        if (other == eraserCollider)
        {
            isBeingErased = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == eraserCollider)
        {
            isBeingErased = false;
        }
    }
    
    // ------------------ Cuttable ------------------
    private void CheckCuttable()
    {
        if (isCut || cutterCollider == null || cuttableCollider == null) return;

        if (cutterCollider.IsTouching(cuttableCollider))
        {
            isCut = true;

            Transform cutter = cutterCollider.transform.parent;
            if (cutter != null)
            {
                Animator anim = cutter.GetComponent<Animator>();
                if (anim != null)
                    anim.SetTrigger("Cut");
            }

            PlayCutFeedback(() =>
            {
                StartFall();
                currentMode = Mode.None;
            });
        }
    }
    
    // ------------------ Draggable ------------------
    void OnMouseDown()
    {
        if (currentMode == Mode.Draggable || currentMode == Mode.Detachable)
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

        if (currentMode == Mode.Draggable)
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

            if (distance < detachThreshold)
            {
                if (detachVisualMode == DetachVisualMode.Scalable)
                {
                    transform.DOMove(detachOrigin, 0.3f).SetEase(Ease.OutBack);
                    transform.DOScale(initialScale, 0.3f).SetEase(Ease.OutBack);
                }
                else if (detachVisualMode == DetachVisualMode.Animated)
                {
                    // Smooth retour à la première frame
                    int currentFrame = 0;
                    if (animatedDetachSprites != null && animatedDetachSprites.Length > 0)
                    {
                        // On récupère la frame actuelle
                        for (int i = 0; i < animatedDetachSprites.Length; i++)
                        {
                            if (spriteRenderer.sprite == animatedDetachSprites[i])
                            {
                                currentFrame = i;
                                break;
                            }
                        }

                        DOVirtual.Float(currentFrame, 0, 0.3f, (val) =>
                        {
                            UpdateAnimatedSprite(val / (animatedDetachSprites.Length - 1f));
                        }).SetEase(Ease.OutBack);
                    }
                }
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

        if (followX)
        {
            float x = worldPos.x;
            if (limitX) x = Mathf.Clamp(x, xLimits.x, xLimits.y);
            targetPos.x = x;
        }

        if (followY)
        {
            float y = worldPos.y;
            if (limitY) y = Mathf.Clamp(y, yLimits.x, yLimits.y);
            targetPos.y = y;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }

    // ------------------ Detachable ------------------
    private void DetachableUpdate()
    {
        if (isDetached) return; // <-- nouveau : bloque toute update si déjà détaché

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        float distance = Vector3.Distance(worldPos, detachOrigin);
        float t = Mathf.Clamp01(distance / detachThreshold);

        switch (detachVisualMode)
        {
            case DetachVisualMode.Scalable:
                Vector2 targetScale = new Vector2(
                    Mathf.Lerp(initialScale.x, maxScale.x, t),
                    Mathf.Lerp(initialScale.y, maxScale.y, t)
                );
                transform.DOScale(new Vector3(targetScale.x, targetScale.y, transform.localScale.z), scaleDuration);
                break;

            case DetachVisualMode.Animated:
                transform.DOKill(false);
                UpdateAnimatedSprite(t);
                break;
        }

        if (t >= 1f)
        {
            isDetached = true; // <-- le sprite est maintenant verrouillé
            isDragging = false;

            // Scalable : reset scale si nécessaire
            if (detachVisualMode == DetachVisualMode.Scalable && !keepDetachedScale)
                transform.DOScale(initialScale, 0.1f);

            // Pour Animated, on ne fait plus de DOScale ni DOMove, il reste à sa position
            if (detachVisualMode == DetachVisualMode.Scalable)
                transform.position = worldPos;

            // Définition du mode après detach
            switch (detachType)
            {
                case DetachType.Fall:
                    StartFall();
                    currentMode = Mode.None;
                    break;

                case DetachType.Draggable:
                    currentMode = Mode.Draggable;
                    break;

                case DetachType.None:
                    currentMode = Mode.None;
                    break;
            }
        }
    }


    // ------------------ Animated ------------------

    private void UpdateAnimatedSprite(float t)
    {
        if (animatedDetachSprites == null || animatedDetachSprites.Length == 0 || spriteRenderer == null)
            return;

        int frameIndex = Mathf.RoundToInt(
            Mathf.Lerp(0, animatedDetachSprites.Length - 1, t)
        );

        frameIndex = Mathf.Clamp(frameIndex, 0, animatedDetachSprites.Length - 1);

        spriteRenderer.sprite = animatedDetachSprites[frameIndex];
    }

    /// ------------------ Fall ------------------
    private void StartFall()
    {
        Vector3 fallTarget = transform.position - new Vector3(0, fallDistance, 0);
        Tween moveTween = transform.DOMove(fallTarget, fallDuration).SetEase(Ease.InQuad);

        if (!keepDetachedScale)
        {
            transform.DOScale(initialScale, fallDuration).SetEase(Ease.OutBack);
        }
    }

    // ------------------ Fillable ------------------
    private Vector3 fillStartLocalPos;

    private void FillableUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fillStartLocalPos = transform.localPosition;
        }

        if (!Input.GetMouseButton(0))
        {
            fillTween?.Kill();
            fillTween = null;
            return;
        }

        Vector3 targetLocalPos = fillStartLocalPos;
        if (fillX) targetLocalPos.x += fillOffsetX;
        if (fillY) targetLocalPos.y += fillOffsetY;

        if (fillTween == null || !fillTween.IsActive())
        {
            fillTween = transform.DOLocalMove(targetLocalPos, fillDuration).SetEase(fillEase);
        }
    }
    
    // ------------------ Erasable ------------------
    private void ErasableUpdate()
    {
        if (!isBeingErased || spriteRenderer == null) return;

        Color c = spriteRenderer.color;
        c.a -= eraseSpeed * Time.deltaTime;
        c.a = Mathf.Clamp(c.a, minAlpha, 1f);
        spriteRenderer.color = c;

        if (c.a <= minAlpha)
        {
            Destroy(gameObject);
        }
    }
    
    // ------------------ Stoppable ------------------
    void StartStoppable()
    {
        stoppableOrigin = transform.position;
        Vector3 target = stoppableOrigin + stoppableTargetOffset;

        stoppableTween = transform.DOMove(target, stoppableDuration)
            .SetEase(stoppableEase)
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    void StoppableUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            stoppableTween?.Kill(false);
            currentMode = Mode.None;
            isStopped = true;
        }
    }
    
    private void PlayCutFeedback(System.Action onComplete)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(initialScale * 1.2f, 0.15f).SetEase(Ease.OutQuad)) // micro squash
            .Append(transform.DOScale(initialScale * 0.8f, 0.15f).SetEase(Ease.InQuad))  // snap back
            .Join(transform.DOShakePosition(
                0.2f,
                strength: new Vector3(0.5f, 0.5f, 0),
                vibrato: 20,
                randomness: 90,
                fadeOut: true))
            .AppendCallback(() => onComplete?.Invoke());
    }

}