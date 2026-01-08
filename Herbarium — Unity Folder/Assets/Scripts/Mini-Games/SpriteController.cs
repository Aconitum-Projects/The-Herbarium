using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.U2D.Animation;

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
        Stoppable,
        Moving
    }

    public enum DetachType
    {
        None,
        Draggable,
        Fall
    }
    
    [System.Flags]
    public enum DetachVisualMode
    {
        None = 0,
        Scalable = 1 << 0,
        Animated = 1 << 1
    }
    
    [System.Flags]
    public enum ShakeVisualMode
    {
        None = 0,
        Animated = 1
    }

    // Mode
    public Mode currentMode = Mode.None;

    // Follow Mouse Settings
    public bool followX = true;
    public bool followY = true;
    public float followSpeed = 20f;
    public Collider2D cutterCollider;
    public bool limitX = false;
    public Vector2 xLimits = new Vector2(-5f, 5f);
    public bool limitY = false;
    public Vector2 yLimits = new Vector2(-3f, 3f);
    public bool useFollowPoints = false;
    public List<Collider2D> followPoints;
    public int passesPerPoint = 3;
    public bool followValidated = false;
    public bool rotateInsteadOfFollow = false;
    public float rotationSpeedFollow = 5f;

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
    public Sprite[] animatedDetachSprites;

    // Cuttable Settings
    public Collider2D cuttableCollider;
    public bool isCut = false;

    // Shakeable Settings
    public bool isShakeable = false;
    public float shakeThreshold = 0.1f;
    public ShakeVisualMode shakeVisualMode;
    public Sprite[] animatedShakeSprites;
    public bool shakeValidated = false;

    // Fillable Settings
    public bool fillPosition = true;
    public bool fillRotation = false;
    public bool fillScale = false;
    public Vector3 fillOffsetPos = Vector3.zero; 
    public float fillOffsetRot = 0f;
    public Vector3 fillOffsetScale = Vector3.one;
    public float fillDuration = 1.2f;
    public Ease fillEase = Ease.InOutSine;
    public bool resetPositionWhenReleased = false;
    
    // Erasable Settings
    public float eraseSpeed = 0.4f;
    public float minAlpha = 0f;
    public BoxCollider2D eraserCollider;
    
    // Stoppable Settings
    public Vector3 stoppableTargetOffset;
    public float stoppableDuration = 1f;
    public Ease stoppableEase = Ease.InOutSine;
    public bool isStopped = false;
    public float stoppableInputDelay = 0.5f;
    
    // Mode Draggable
    public bool rotateInsteadOfMove = false;
    public float rotationSpeed = 5f;
    public bool limitRotation = false;
    public float minRotation = -45f;
    public float maxRotation = 45f;
    
    // Moving Settings
    public Vector2 moveDirection = Vector2.right;
    public Vector2 speedRange = new Vector2(1f, 3f);
    public Collider2D collectCollider;
    public Collider2D destroyCollider;
    public bool collected = false;
    public bool destroyed = false;
    public SpriteRenderer collectedSpriteRenderer;

    // Private
    float currentMoveSpeed;
    Vector3 stoppableOrigin;
    Tween stoppableTween;
    SpriteRenderer spriteRenderer;
    bool isBeingErased = false;
    Vector3 lastPos;
    Vector3 initialScale;
    Vector3 detachOrigin;
    Camera mainCam;
    Vector3 offset;
    bool isDragging = false;
    Tween fillTween;
    Vector3 fillStartPos;
    SpriteRenderer targetSpriteRenderer;
    bool canStop = false;
    Vector3 lastMousePos;
    float shakeAnimT = 0f;
    Tween shakeRewindTween;
    float currentShakeSpeed = 0f;
    Vector3 fillStartLocalPos;
    float fillStartRotation;
    Vector3 fillStartScale;
    Vector3 fillStartPosGlobal;
    bool fillInitialized = false;
    int[] pointPasses;    
    
    void Awake()
    {
        mainCam = Camera.main;
        initialScale = transform.localScale;
        lastPos = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        targetSpriteRenderer = spriteRenderer != null
            ? spriteRenderer
            : GetComponentInChildren<SpriteRenderer>();
        
        fillStartPosGlobal = transform.position;
        fillStartRotation = transform.eulerAngles.z;
        fillStartScale = transform.localScale;

        if (useFollowPoints && followPoints != null && followPoints.Count > 0)
        {
            pointPasses = new int[followPoints.Count];
        }
    }

    void OnEnable()
    {
        if (currentMode == Mode.Stoppable)
            StartStoppable();

        if (currentMode == Mode.Moving)
            InitMoving();
    }
    void InitMoving()
    {
        currentMoveSpeed = Random.Range(speedRange.x, speedRange.y);
        moveDirection = moveDirection.normalized;
    }

    void OnDisable()
    {
        stoppableTween?.Kill();
        canStop = false;
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
            case Mode.Moving:
                MovingUpdate();
                break;
        }

        // ----------- Shake Detection -----------
        if (isShakeable)
        {
            Vector3 delta = transform.position - lastPos;
            currentShakeSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

            // Validation du shake
            if (!shakeValidated && currentShakeSpeed >= shakeThreshold)
            {
                shakeValidated = true;
            }

            // Animation (optionnelle)
            if (shakeVisualMode.HasFlag(ShakeVisualMode.Animated))
            {
                HandleShakeAnimated();
            }
        }

        lastPos = transform.position;


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // --- Erasable ---
        if (currentMode == Mode.Erasable && eraserCollider != null)
        {
            if (other == eraserCollider)
                isBeingErased = true;
        }

        // --- Moving ---
        if (currentMode == Mode.Moving)
        {
            if (!collected && collectCollider != null && other == collectCollider)
            {
                collected = true;
                currentMode = Mode.FollowMouse;
            }

            else if (!destroyed && destroyCollider != null && other == destroyCollider)
            {
                destroyed = true;
                Destroy(gameObject);
            }
        }
        
        // --- Follow Points ---
        if (useFollowPoints && followPoints != null && followPoints.Contains(other))
        {
            int index = followPoints.IndexOf(other);
            if (pointPasses != null && index >= 0 && index < pointPasses.Length)
            {
                pointPasses[index]++;

                followValidated = true;
                for (int i = 0; i < pointPasses.Length; i++)
                {
                    if (pointPasses[i] < passesPerPoint)
                    {
                        followValidated = false;
                        break;
                    }
                }
            }
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
            lastMousePos = Input.mousePosition;
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

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        if (currentMode == Mode.Draggable)
        {
            if (!rotateInsteadOfMove)
            {
                // DRAG CLASSIQUE
                transform.position = worldPos + offset;
            }
            else
            {
                // ROTATION
                Vector3 mouseDelta = Input.mousePosition - lastMousePos;
                float angle = mouseDelta.x * rotationSpeed * Time.deltaTime;

                if (limitRotation)
                {
                    float currentZ = transform.eulerAngles.z;
                    if (currentZ > 180f) currentZ -= 360f;

                    float clampedZ = Mathf.Clamp(currentZ + angle, minRotation, maxRotation);
                    angle = clampedZ - currentZ;
                }

                transform.Rotate(0, 0, angle);
            }
        }
        else if (currentMode == Mode.Detachable)
        {
            DetachableUpdate();
        }

        lastMousePos = Input.mousePosition;
    }
    void OnMouseUp()
    {
        if (currentMode == Mode.Detachable && isDragging)
        {
            float distance = Vector3.Distance(transform.position, detachOrigin);

            if (distance < detachThreshold)
            {
                bool useScale = detachVisualMode.HasFlag(DetachVisualMode.Scalable);
                bool useAnim  = detachVisualMode.HasFlag(DetachVisualMode.Animated);
                
                
                if (useScale)
                {
                    transform.DOMove(detachOrigin, 0.3f).SetEase(Ease.OutBack);
                    transform.DOScale(initialScale, 0.3f).SetEase(Ease.OutBack);
                }
                
                if (useAnim)
                {
                    int currentFrame = 0;
                    if (animatedDetachSprites != null && animatedDetachSprites.Length > 0)
                    {
                        for (int i = 0; i < animatedDetachSprites.Length; i++)
                        {
                            if (targetSpriteRenderer.sprite == animatedDetachSprites[i])
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

    // ------------------ Shake Animated ------------------
    void HandleShakeAnimated()
    {
        if (shakeValidated)
        {
            UpdateShakeAnimatedSprite(1f);
            return;
        }

        float normalizedShake =
            (currentShakeSpeed - shakeThreshold) / shakeThreshold;

        normalizedShake = Mathf.Clamp01(normalizedShake);

        shakeAnimT = Mathf.Lerp(
            shakeAnimT,
            normalizedShake,
            Time.deltaTime * 5f
        );

        if (shakeAnimT >= 0.98f)
        {
            shakeValidated = true;
            shakeAnimT = 1f;
        }

        UpdateShakeAnimatedSprite(shakeAnimT);
    }
    void UpdateShakeAnimatedSprite(float t)
    {
        if (animatedShakeSprites == null ||
            animatedShakeSprites.Length == 0 ||
            targetSpriteRenderer == null)
            return;

        int frame = Mathf.RoundToInt(
            Mathf.Lerp(0, animatedShakeSprites.Length - 1, t)
        );

        frame = Mathf.Clamp(frame, 0, animatedShakeSprites.Length - 1);
        targetSpriteRenderer.sprite = animatedShakeSprites[frame];
    }

    // ------------------ Follow Mouse ------------------
    private void FollowMouseUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        if (!rotateInsteadOfFollow)
        {
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
        else
        {
            Vector3 direction = worldPos - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float z = Mathf.LerpAngle(transform.eulerAngles.z, angle, rotationSpeedFollow * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, z);
        }

        lastMousePos = Input.mousePosition;
    }


    // ------------------ Detachable ------------------
    private void DetachableUpdate()
    {
        if (isDetached) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z);
        Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

        float distance = Vector3.Distance(worldPos, detachOrigin);
        float t = Mathf.Clamp01(distance / detachThreshold);

        bool useScale = detachVisualMode.HasFlag(DetachVisualMode.Scalable);
        bool useAnim  = detachVisualMode.HasFlag(DetachVisualMode.Animated);

        if (useScale)
        {
            Vector2 targetScale = new Vector2(
                Mathf.Lerp(initialScale.x, maxScale.x, t),
                Mathf.Lerp(initialScale.y, maxScale.y, t)
            );

            transform.DOScale(
                new Vector3(targetScale.x, targetScale.y, transform.localScale.z),
                scaleDuration
            );
        }

        if (useAnim)
        {
            UpdateAnimatedSprite(t);
        }

        if (t >= 1f)
        {
            isDetached = true;
            isDragging = false;

            if (useScale)
            {
                transform.DOKill(false);

                if (!keepDetachedScale)
                {
                    transform.DOScale(initialScale, 0.15f)
                        .SetEase(Ease.OutBack);
                }
            }

            if (useScale)
                transform.position = ClampToScreen(worldPos);

            // ----------- NOUVEAU -----------
            BoneHanging2D boneScript = GetComponent<BoneHanging2D>();
            if (boneScript != null)
                boneScript.enabled = false;
            // -------------------------------

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
        if (animatedDetachSprites == null || animatedDetachSprites.Length == 0 || targetSpriteRenderer == null)
            return;

        int frameIndex = Mathf.RoundToInt(
            Mathf.Lerp(0, animatedDetachSprites.Length - 1, t)
        );

        frameIndex = Mathf.Clamp(frameIndex, 0, animatedDetachSprites.Length - 1);

        targetSpriteRenderer.sprite = animatedDetachSprites[frameIndex];
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
    private void FillableUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !fillInitialized)
        {
            fillStartPosGlobal = transform.position;
            fillStartRotation = transform.eulerAngles.z;
            fillStartScale = transform.localScale;
            fillInitialized = true;
        }

        if (!Input.GetMouseButton(0))
        {
            if (!fillInitialized) return;

            if (resetPositionWhenReleased)
            {
                fillTween?.Kill();
                Sequence resetSeq = DOTween.Sequence();
                if (fillPosition) resetSeq.Join(transform.DOMove(fillStartPosGlobal, fillDuration).SetEase(fillEase));
                if (fillRotation) resetSeq.Join(transform.DORotate(new Vector3(0,0,fillStartRotation), fillDuration).SetEase(fillEase));
                if (fillScale) resetSeq.Join(transform.DOScale(fillStartScale, fillDuration).SetEase(fillEase));
                fillTween = resetSeq;
            }
            else
            {
                fillTween?.Kill();
                fillTween = null;
            }

            return;
        }

        Vector3 targetPos = fillStartPosGlobal + (fillPosition ? fillOffsetPos : Vector3.zero);
        float targetRot = fillStartRotation + (fillRotation ? fillOffsetRot : 0f);
        Vector3 targetScale = fillStartScale;
        if (fillScale)
        {
            targetScale = new Vector3(
                fillStartScale.x * fillOffsetScale.x,
                fillStartScale.y * fillOffsetScale.y,
                fillStartScale.z * fillOffsetScale.z
            );
        }

        fillTween?.Kill();

        Sequence seq = DOTween.Sequence();
        if (fillPosition) seq.Join(transform.DOMove(targetPos, fillDuration).SetEase(fillEase));
        if (fillRotation) seq.Join(transform.DORotate(new Vector3(0,0,targetRot), fillDuration).SetEase(fillEase));
        if (fillScale) seq.Join(transform.DOScale(targetScale, fillDuration).SetEase(fillEase));
        fillTween = seq;

        if (fillScale)
        {
            transform.localScale = new Vector3(
                Mathf.Min(transform.localScale.x, targetScale.x),
                Mathf.Min(transform.localScale.y, targetScale.y),
                Mathf.Min(transform.localScale.z, targetScale.z)
            );
        }
    }
    
    // ------------------ Erasable ------------------
    private void ErasableUpdate()
    {
        if (!isBeingErased || targetSpriteRenderer == null) return;

        Color c = targetSpriteRenderer.color;
        c.a -= eraseSpeed * Time.deltaTime;
        c.a = Mathf.Clamp(c.a, minAlpha, 1f);
        targetSpriteRenderer.color = c;

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

        canStop = false;
        DOVirtual.DelayedCall(stoppableInputDelay, () => canStop = true);
    }
    void StoppableUpdate()
    {
        if (!canStop) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            stoppableTween?.Kill(false);
            currentMode = Mode.None;
            isStopped = true;
        }
    }
    
    void MovingUpdate()
    {
        if (collected || destroyed) return;

        transform.position +=
            (Vector3)(moveDirection * currentMoveSpeed * Time.deltaTime);
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
    private Vector3 ClampToScreen(Vector3 worldPos)
    {
        Vector3 vp = mainCam.WorldToViewportPoint(worldPos);

        vp.x = Mathf.Clamp01(vp.x);
        vp.y = Mathf.Clamp01(vp.y);

        Vector3 clampedWorld = mainCam.ViewportToWorldPoint(vp);
        clampedWorld.z = worldPos.z;

        return clampedWorld;
    }


}