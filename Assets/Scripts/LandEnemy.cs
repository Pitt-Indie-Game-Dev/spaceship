using UnityEngine;
using UnityEngine.UIElements;

public class LandEnemy : MonoBehaviour
{
    private bool inShootingRange;
    private bool inPunchingRange;
    private Rigidbody2D rb;
    public int health; // player can't attack yet but still putting this here
    public float movementSpeed;
    public float punchSpeed; // time between punches
    private float punchCooldown = 1.5f;
    public float shootSpeed; // time between shots
    private float shootCooldown = 1.5f;
    public Transform player;
    Vector3 playerPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerPosition = player.position;
        if (-1f < playerPosition.x - transform.position.x && playerPosition.x - transform.position.x < 1f)
        {
            rb.linearVelocityX = 0f;
            if (punchCooldown <= 0) 
            {
                Punch();
                punchCooldown = punchSpeed;
            }
            else
            {
                punchCooldown -= Time.deltaTime;
            }
        }
        else if (-4f < playerPosition.x - transform.position.x && playerPosition.x - transform.position.x < 4f)
        {
            rb.linearVelocityX = 0f;
            if (shootCooldown <= 0) 
            {
                Shoot();
                shootCooldown = shootSpeed;
            }
            else
            {
                shootCooldown -= Time.deltaTime;
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
        print("SHOOT");
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

    void Die() // when we have a gun in the game
    {
        
    }
}
