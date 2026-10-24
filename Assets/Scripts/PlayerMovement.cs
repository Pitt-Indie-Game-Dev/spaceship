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

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = Anims.GetComponent<Animator>();
        feet = new FootBounds(GetComponent<BoxCollider2D>());
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
        body.linearVelocity = new Vector2(horizontal * speed, body.linearVelocity.y);

        bool grounded = IsGrounded();
        bool isFalling = !grounded && body.linearVelocity.y < 0f;
        bool isCrouching = kb.cKey.IsPressed();
        bool isRunning = grounded && !isCrouching && kb.shiftKey.IsPressed();
        bool isRolling = isCrouching && kb.shiftKey.IsPressed();

        animator.SetBool("isMoving", horizontal != 0);
        animator.SetBool("isGrounded", grounded);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isRolling", isRolling);

        if (horizontal != 0)
            Anims.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);

        if (isRunning) 
            body.linearVelocity *= Vector2.right * 1.5f;

        if (grounded && kb.spaceKey.isPressed) 
            body.linearVelocity = new(body.linearVelocity.x, speed);
    }
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(feet.Origin, feet.Size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(feet.Origin, feet.Size);
    }
}
