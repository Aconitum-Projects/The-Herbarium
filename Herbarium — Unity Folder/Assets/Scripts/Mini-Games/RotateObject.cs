using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0f, 180f, 0f); // degrés par seconde

    void Update()
    {
        // Rotation continue
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}