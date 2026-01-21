using UnityEngine;

public class Projection : MonoBehaviour
{
    [Header("Sprite Settings")]
    public GameObject[] spritePrefabs;
    public float lifetime = 3f;
    public float growSpeed = 5f;

    [Header("Particle Emission")]
    public int particlesPerShot = 1;
    public float spreadAngle = 15f;

    [Header("Shooting Settings")]
    public Vector2 direction = Vector2.right;
    public float speed = 5f;
    public float speedRandomness = 1f;
    public float fireRate = 0.05f;

    [Header("Trigger Mode")]
    public bool dropOnMovement = false;
    public float movementThreshold = 0.01f;

    [Header("Reference Object")]
    public Transform referenceObject; // L'objet à suivre pour générer les particules

    private float fireCooldown;
    private Vector3 lastReferencePos;
    private Vector2 movementDirection;

    void Start()
    {
        if (referenceObject != null)
            lastReferencePos = referenceObject.position;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        bool shouldShoot = false;

        if (dropOnMovement && referenceObject != null)
        {
            Vector3 delta = referenceObject.position - lastReferencePos;
            if (delta.magnitude > movementThreshold)
            {
                movementDirection = delta.normalized;
                shouldShoot = true;
            }
            lastReferencePos = referenceObject.position;
        }
        else
        {
            shouldShoot = Input.GetMouseButton(0);
        }

        if (shouldShoot && fireCooldown <= 0f)
        {
            EmitParticles();
            fireCooldown = fireRate;
        }
    }

    void EmitParticles()
    {
        for (int i = 0; i < particlesPerShot; i++)
        {
            ShootSprite();
        }
    }

    void ShootSprite()
    {
        if (spritePrefabs.Length == 0) return;

        GameObject prefab = spritePrefabs[Random.Range(0, spritePrefabs.Length)];
        GameObject particle = Instantiate(prefab, transform.position, Quaternion.identity);

        Vector2 baseDir = dropOnMovement && referenceObject != null ? movementDirection : direction.normalized;

        float angleOffset = Random.Range(-spreadAngle, spreadAngle);
        Vector2 finalDir = Quaternion.Euler(0, 0, angleOffset) * baseDir;

        Rigidbody2D rb = particle.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 velocity =
                finalDir * speed +
                Random.insideUnitCircle * speedRandomness;

            rb.linearVelocity = velocity;
        }

        particle.transform.localScale = Vector3.zero;
        particle.AddComponent<SpriteGrow>().growSpeed = growSpeed;

        Destroy(particle, lifetime);
    }
}

public class SpriteGrow : MonoBehaviour
{
    public float growSpeed = 5f;

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            Vector3.one,
            growSpeed * Time.deltaTime
        );
    }
}
