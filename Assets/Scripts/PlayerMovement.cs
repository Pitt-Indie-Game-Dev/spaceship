using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform Anims;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = Anims.GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        float horizontal = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f:0f) - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f: 0f);
        body.linearVelocity = new Vector2(horizontal*speed, body.linearVelocity.y);

        bool grounded = IsGrounded();
        bool isFalling = !grounded && body.linearVelocity.y < 0f;
        bool isCrouching = kb.cKey.IsPressed();

        animator.SetBool("isMoving", horizontal != 0);
        animator.SetBool("isGrounded", grounded);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isCrouching", isCrouching);

        if (horizontal != 0)
            Anims.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);


        if (kb.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, speed);
        }
        
        
    }
    private bool IsGrounded()
    {
        Vector2 size = new Vector2(boxCollider.bounds.size.x * 0.8f, 0.1f);
        Vector2 origin = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        Vector2 size = new Vector2(boxCollider.bounds.size.x * 0.8f, 0.1f);
        Vector2 origin = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, size);
    }
}
