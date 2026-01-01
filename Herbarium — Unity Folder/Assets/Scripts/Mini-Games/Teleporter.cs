using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Collider2D toTeleportCollider;
    public Collider2D triggerCollider;
    public Vector2 newPosition;

    private void Update()
    {
        if (toTeleportCollider.IsTouching(triggerCollider))
        { 
            transform.position = newPosition;
        }
    }
}