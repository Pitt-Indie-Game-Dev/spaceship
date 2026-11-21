using UnityEngine;

public class LandEnemy : MonoBehaviour
{
    private Rigidbody2D rb;
    public int health; 
    public float movementSpeed = 3f;
    public float punchSpeed = 1.5f; // time between punches
    private float punchCooldown = 1.5f;
    public float shootSpeed = 1.5f; // time between shots
    private float shootCooldown = 1.5f;
    public float bulletSpeed = 10f;
    
    public Transform player;
    public GameObject bulletPrehab;
    private Vector3 playerPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return; // Prevents errors if the player is destroyed

        playerPosition = player.position;
        float distanceX = playerPosition.x - transform.position.x;

        // Flip the enemy to face the player
        if (distanceX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(distanceX);
            transform.localScale = scale;
        }

        if (-1f < distanceX && distanceX < 1f)
        {
            rb.linearVelocityX = 0f;
            if (punchCooldown <= 0) 
            {
                Punch();
                punchCooldown = punchSpeed;
            }
            else
            {
                punchCooldown -= Time.fixedDeltaTime;
            }
        }
        else if (-4f < distanceX && distanceX < 4f)
        {
            rb.linearVelocityX = 0f;
            if (shootCooldown <= 0) 
            {
                Shoot();
                shootCooldown = shootSpeed;
            }
            else
            {
                shootCooldown -= Time.fixedDeltaTime;
            }
        }
        else
        {
            ApproachPlayer();
        }
    }

    void Punch()
    {
        print("PUNCH");
    }

    void Shoot()
    {
        // 1. Calculate exact direction vector toward the player
        Vector2 shootDirection = (playerPosition - transform.position).normalized;

        // 2. Calculate angle in degrees and rotate the bullet (handles left/right/diagonal automatically)
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);

        // 3. Spawn, launch, and clean up the bullet
        GameObject bullet = Instantiate(bulletPrehab, transform.position, bulletRotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;
        Destroy(bullet, 3f);
    }

    void ApproachPlayer()
    {
        if (playerPosition.x - transform.position.x > 0)
        {
            rb.linearVelocityX = movementSpeed;
        }
        else
        {
            rb.linearVelocityX = -movementSpeed;
        }
    }

    void Die() 
    {
        
    }
}
