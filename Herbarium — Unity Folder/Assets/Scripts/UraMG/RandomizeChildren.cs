using UnityEngine;

public class RandomizeChildren : MonoBehaviour
{
    [Header("Position (Local)")]

    [Tooltip(
        "Offset minimum appliqué à la position locale des enfants.\n" +
        "Les valeurs sont tirées aléatoirement entre Min et Max pour chaque axe."
    )]
    public Vector3 positionMin = new Vector3(-0.05f, -0.05f, 0f);

    [Tooltip(
        "Offset maximum appliqué à la position locale des enfants.\n" +
        "Utilise de petites valeurs pour éviter les cassures visuelles."
    )]
    public Vector3 positionMax = new Vector3(0.05f, 0.05f, 0f);


    [Header("Rotation (Local)")]

    [Tooltip(
        "Rotation minimale autorisée (en degrés) autour de la rotation d'origine.\n" +
        "Clamp appliqué par axe."
    )]
    public Vector3 rotationMin = new Vector3(0f, 0f, -15f);

    [Tooltip(
        "Rotation maximale autorisée (en degrés) autour de la rotation d'origine.\n" +
        "Clamp appliqué par axe."
    )]
    public Vector3 rotationMax = new Vector3(0f, 0f, 15f);


    void Awake()
    {
        foreach (Transform child in transform)
        {
            // --- Position ---
            Vector3 randomPos = new Vector3(
                Random.Range(positionMin.x, positionMax.x),
                Random.Range(positionMin.y, positionMax.y),
                Random.Range(positionMin.z, positionMax.z)
            );

            child.localPosition += randomPos;

            // --- Rotation ---
            Vector3 randomRot = new Vector3(
                Random.Range(rotationMin.x, rotationMax.x),
                Random.Range(rotationMin.y, rotationMax.y),
                Random.Range(rotationMin.z, rotationMax.z)
            );

            child.localRotation *= Quaternion.Euler(randomRot);
        }
    }
}