using UnityEngine;

public class BetterDraggable : MonoBehaviour
{
    // =========================
    // DRAG SORTING (HOVER)
    // =========================

    [Header("Drag Sorting")]

    [Tooltip(
        "SpriteRenderer à mettre en avant lors du drag.\n" +
        "Si null, le script cherchera un SpriteRenderer sur cet objet ou ses parents.\n" +
        "Permet aussi de déclencher le hover depuis un bone."
    )]
    public SpriteRenderer targetRenderer;

    [Tooltip(
        "Offset ajouté à l'ordre de rendu pendant le drag."
    )]
    public int hoverOrderOffset = 100;

    [Tooltip(
        "Offset appliqué après relâchement si le bone est dans la zone.\n" +
        "Ajouté à l'ordre d'origine du sprite."
    )]
    public int zoneOrderOffset = 30;

    [Header("Zone Detection")]

    [Tooltip(
        "Collider de zone dans lequel le bone doit être posé\n" +
        "pour conserver un ordre d'affichage supérieur."
    )]
    public Collider2D placementZone;

    int baseSortingOrder;
    bool isHovering;
    bool isInsideZone;


    // =========================
    // SOFT BONES
    // =========================

    [Header("Soft Bones Motion")]

    public float influence = 25f;
    public float damping = 10f;
    public float returnStrength = 15f;
    public float maxAngle = 25f;

    Transform[] bones;
    Quaternion[] restRotations;
    float[] angularVelocity;
    Vector3 lastRootPos;


    // =========================
    // LIFECYCLE
    // =========================

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInParent<SpriteRenderer>();

        if (targetRenderer != null)
            baseSortingOrder = targetRenderer.sortingOrder;

        bones = GetComponentsInChildren<Transform>();
        restRotations = new Quaternion[bones.Length];
        angularVelocity = new float[bones.Length];

        for (int i = 0; i < bones.Length; i++)
            restRotations[i] = bones[i].localRotation;

        lastRootPos = transform.position;
    }

    void Update()
    {
        UpdateSoftBones();
    }


    // =========================
    // DRAG EVENTS
    // =========================

    void OnMouseDown()
    {
        SetHover(true);
    }

    void OnMouseUp()
    {
        SetHover(false);
        ApplyZoneSorting();
    }

    void OnDisable()
    {
        SetHover(false);
        ApplyZoneSorting();
    }

    void SetHover(bool state)
    {
        if (targetRenderer == null) return;
        if (isHovering == state) return;

        isHovering = state;

        if (state)
            targetRenderer.sortingOrder = baseSortingOrder + hoverOrderOffset;
    }

    void ApplyZoneSorting()
    {
        if (targetRenderer == null) return;

        targetRenderer.sortingOrder = isInsideZone
            ? baseSortingOrder + zoneOrderOffset
            : baseSortingOrder;
    }


    // =========================
    // ZONE DETECTION
    // =========================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (placementZone != null && other == placementZone)
            isInsideZone = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (placementZone != null && other == placementZone)
            isInsideZone = false;
    }


    // =========================
    // SOFT BONES UPDATE
    // =========================

    void UpdateSoftBones()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 rootVelocity = (transform.position - lastRootPos) / dt;
        lastRootPos = transform.position;

        float moveAmount = rootVelocity.magnitude;

        for (int i = 1; i < bones.Length; i++)
        {
            float depth = (float)i / (bones.Length - 1);

            if (moveAmount > 0.001f)
            {
                float force = Vector3.Dot(rootVelocity, transform.right);
                angularVelocity[i] += force * influence * depth * dt;
            }

            angularVelocity[i] *= Mathf.Exp(-damping * dt);
            bones[i].localRotation *= Quaternion.Euler(0, 0, angularVelocity[i]);

            Quaternion rest = restRotations[i];
            Quaternion current = bones[i].localRotation;
            Quaternion delta = current * Quaternion.Inverse(rest);

            delta.ToAngleAxis(out float angle, out Vector3 axis);
            angle = Mathf.DeltaAngle(0f, angle);
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

            bones[i].localRotation = Quaternion.AngleAxis(angle, axis) * rest;

            if (moveAmount < 0.001f)
            {
                bones[i].localRotation = Quaternion.Slerp(
                    bones[i].localRotation,
                    rest,
                    returnStrength * dt
                );
            }
        }
    }
}
