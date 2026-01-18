using UnityEngine;

public class Projection : MonoBehaviour
{
    [Header("Sprite Settings")]
    public GameObject[] spritePrefabs;
    public float lifetime = 3f;
    public float growSpeed = 5f;

    [Header("Shooting Settings")]
    public Vector2 direction = Vector2.right;
    public float speed = 5f;
    public float speedRandomness = 1f;
    public float fireRate = 0.2f;

    [Header("Rotation Settings")]
    public float minRotation = 0f;
    public float maxRotation = 0f;

    private float fireCooldown = 0f;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            ShootSprite();
            fireCooldown = fireRate;
        }
    }

    void ShootSprite()
    {
        if (spritePrefabs.Length == 0) return;
        GameObject chosenPrefab = spritePrefabs[Random.Range(0, spritePrefabs.Length)];

        // Instantiate
        GameObject newSprite = Instantiate(chosenPrefab, transform.position, Quaternion.identity);

        float randomRotation = Random.Range(minRotation, maxRotation);
        newSprite.transform.rotation = Quaternion.Euler(0, 0, randomRotation);

        Vector2 finalDirection = direction.normalized;
        finalDirection = Quaternion.Euler(0, 0, randomRotation) * finalDirection;

        Vector2 speedVariation = new Vector2(
            Random.Range(-speedRandomness, speedRandomness),
            Random.Range(-speedRandomness, speedRandomness)
        );

        Vector2 finalVelocity = finalDirection * speed + speedVariation;

        Rigidbody2D rb = newSprite.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = finalVelocity;
        }
        
        newSprite.transform.localScale = Vector3.zero;
        newSprite.AddComponent<SpriteGrow>().growSpeed = growSpeed;

        Destroy(newSprite, lifetime);
    }
}

public class SpriteGrow : MonoBehaviour
{
    public float growSpeed = 5f;

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, growSpeed * Time.deltaTime);
    }
}
