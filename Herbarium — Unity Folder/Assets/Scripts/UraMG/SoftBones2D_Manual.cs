using UnityEngine;

public class SoftBones2D_Manual : MonoBehaviour
{
    [Header("Motion")]

    [Tooltip(
        "Intensité de la réaction des bones au déplacement du root.\n" +
        "Plus la valeur est élevée, plus les bones se plient fortement quand le sprite bouge.\n" +
        "Agit comme une force appliquée par la vitesse du root.\n" +
        "Valeurs typiques : 10 (souple) → 30 (très réactif)."
    )]
    public float influence = 25f;

    [Tooltip(
        "Amortissement de la rotation (perte d'énergie).\n" +
        "Contrôle à quelle vitesse le mouvement se stabilise.\n" +
        "Valeur élevée = mouvement court et sec.\n" +
        "Valeur basse = oscillations longues et flottantes.\n" +
        "Valeurs typiques : 6 (flottant) → 12 (stable)."
    )]
    public float damping = 10f;

    [Tooltip(
        "Force de rappel vers la rotation de repos.\n" +
        "Plus cette valeur est élevée, plus les bones reviennent vite\n" +
        "à leur pose d'origine quand le root s'arrête.\n" +
        "Ne génère aucun mouvement si le sprite est immobile.\n" +
        "Valeurs typiques : 8 (lent) → 18 (snappy)."
    )]
    public float returnStrength = 15f;

    [Tooltip(
        "Angle maximal autorisé par bone, autour de sa rotation de repos.\n" +
        "Empêche toute déformation excessive ou cassure du sprite.\n" +
        "La rotation est clampée symétriquement (+/- angle).\n" +
        "Valeurs typiques : 10° (rigide) → 30° (très souple)."
    )]
    public float maxAngle = 25f;

    Transform[] bones;
    Quaternion[] restRotations;
    float[] angularVelocity;

    Vector3 lastRootPos;

    void Awake()
    {
        bones = GetComponentsInChildren<Transform>();
        restRotations = new Quaternion[bones.Length];
        angularVelocity = new float[bones.Length];

        for (int i = 0; i < bones.Length; i++)
            restRotations[i] = bones[i].localRotation;

        lastRootPos = transform.position;
    }

    void Update()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 rootVelocity = (transform.position - lastRootPos) / dt;
        lastRootPos = transform.position;

        float moveAmount = rootVelocity.magnitude;

        for (int i = 1; i < bones.Length; i++)
        {
            float depth = (float)i / (bones.Length - 1);

            // 1. réaction uniquement si le root bouge
            if (moveAmount > 0.001f)
            {
                float force = Vector3.Dot(rootVelocity, transform.right);
                angularVelocity[i] += force * influence * depth * dt;
            }

            // 2. damping
            angularVelocity[i] *= Mathf.Exp(-damping * dt);

            // 3. appliquer rotation
            bones[i].localRotation *= Quaternion.Euler(0, 0, angularVelocity[i]);

            // 4. clamp autour de la rotation de repos
            Quaternion rest = restRotations[i];
            Quaternion current = bones[i].localRotation;
            Quaternion delta = current * Quaternion.Inverse(rest);

            delta.ToAngleAxis(out float angle, out Vector3 axis);
            angle = Mathf.DeltaAngle(0f, angle);
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

            bones[i].localRotation = Quaternion.AngleAxis(angle, axis) * rest;

            // 5. retour automatique au repos quand ça ne bouge plus
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
