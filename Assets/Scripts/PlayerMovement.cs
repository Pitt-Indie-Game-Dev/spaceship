using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{   
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform Anims;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;

    private Rigidbody2D body;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = Anims.GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb != null)
        {
            float horizontal = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f) 
                             - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
            
            body.linearVelocity = new Vector2(horizontal * speed, body.linearVelocity.y);

            bool grounded = IsGrounded();
            bool isFalling = !grounded && body.linearVelocity.y < 0f;
            bool isCrouching = kb.cKey.isPressed;

            if (isCrouching)
            {
                capsuleCollider.size = new Vector2(0.5f, 0.9f);
            }
            else
            {
                capsuleCollider.size = new Vector2(0.5f, 1.1f);
            }

            animator.SetBool("isMoving", horizontal != 0);
            animator.SetBool("isGrounded", grounded);
            animator.SetBool("isFalling", isFalling);
            animator.SetBool("isCrouching", isCrouching);

            if (horizontal != 0)
            {
                Anims.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
            }

            if (kb.spaceKey.wasPressedThisFrame && grounded)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // 1. Get the exact world position where the mouse clicked
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f; // Keep it flat on the 2D plane

        // 2. Calculate the exact direction toward the mouse
        Vector2 shootDirection = (mouseWorldPos - transform.position).normalized;

        // 3. Rotate the bullet sprite to point in the direction it's flying
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);

        // 4. Spawn the bullet, launch it, and delete the clone after 3 seconds
        GameObject bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;
        Destroy(bullet, 3f);
    }

    private bool IsGrounded()
    {
        Vector2 size = new Vector2(capsuleCollider.bounds.size.x * 0.8f, 0.1f);
        Vector2 origin = new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        if (capsuleCollider == null) return;
        Vector2 size = new Vector2(capsuleCollider.bounds.size.x * 0.8f, 0.1f);
        Vector2 origin = new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, size);
    }
}
