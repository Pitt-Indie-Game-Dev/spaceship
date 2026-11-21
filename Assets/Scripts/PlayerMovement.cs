using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform Anims;
    private Rigidbody2D body;
    private Animator animator;
    private FootBounds feet;
    private Weapons weapons;
    private BoxCollider2D hitbox;
    private bool headbonk;
    private bool grounded;
    private bool obstructed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = Anims.GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        feet = new FootBounds(hitbox);
        weapons = transform.GetChild(0).GetComponent<Weapons>();
    }

    private class FootBounds
    {
        private readonly Collider2D col;
        public FootBounds(Collider2D col) => this.col = col;

        public Vector2 Origin => new(col.bounds.center.x, col.bounds.min.y);
        public Vector2 Size   => new(col.bounds.size.x * 0.8f, 0.1f);
    }

    private void Update()
    {
        var kb = Keyboard.current;
        float horizontal = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed || kb.leftArrowKey.isPressed  ? 1f : 0f);
        if(grounded || !obstructed) body.linearVelocity = new Vector2(horizontal * speed, body.linearVelocity.y);

        bool isMoving = horizontal != 0;
        bool isFalling = !grounded && body.linearVelocity.y < 0f;
        bool isCrouching = kb.cKey.IsPressed() || headbonk;
        bool armless = isMoving && kb.shiftKey.IsPressed() && !(!grounded && !isCrouching);
        bool isRunning = grounded && !isCrouching && armless;
        bool isRolling = isCrouching && armless;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isGrounded", grounded);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isRolling", isRolling);
        
        weapons.leftRenderer.enabled = weapons.rightRenderer.enabled = !armless;

        if(isCrouching)
        {
            // Anims.localPosition = new(0, -0.12f);
            hitbox.offset = new(0.05f,0);
            hitbox.size = new(0.52f,0.8f);
        }
        else
        {
            // Anims.localPosition = new(0, 0);
            hitbox.offset = new(0.05f,0);
            hitbox.size = new(0.52f,1);
        }

        if (isMoving)
            Anims.localScale = new(Mathf.Sign(horizontal), 1, 1);

        if (isRunning && isMoving) 
            body.linearVelocity *= Vector2.right * 1.5f;

        if (grounded && kb.spaceKey.isPressed) 
            body.linearVelocity = new(body.linearVelocity.x, speed);
    }

    private void FixedUpdate()
    {
        var b = ObstructedCheckBounds();
        obstructed = Physics2D.BoxCast(b.Item1, b.Item2, 0f, Vector2.right, 0.1f, groundLayer).collider != null;
        grounded = Physics2D.BoxCast(feet.Origin, feet.Size, 0f, Vector2.down, 0.1f, groundLayer).collider != null;
        b = HeadBonkBounds();
        headbonk = false;
        foreach (RaycastHit2D h in Physics2D.BoxCastAll(transform.position, b.Item2, 0f, b.Item1)) if (h.collider.transform.gameObject != gameObject)
        {
            headbonk = true;
            break;
        }
    }

    private (Vector2, Vector2) ObstructedCheckBounds() => (
        transform.position + new Vector3(hitbox.bounds.size.x/2 * Anims.localScale.x, 0), 
        new Vector2(0.25f, hitbox.bounds.size.y)
    );

    private (Vector2, Vector2) HeadBonkBounds() => (
        new Vector3(0.05f, 0.37f), 
        new Vector2(0.32f, 0.25f)
    );

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(feet != null) Gizmos.DrawWireCube(feet.Origin, feet.Size);
        if(hitbox)
        {
            var b = ObstructedCheckBounds();
            Gizmos.DrawWireCube(b.Item1, b.Item2);
            b = HeadBonkBounds();
            Gizmos.DrawWireCube(transform.position + (transform.rotation * b.Item1), b.Item2);
        }
    }
}
