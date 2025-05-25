using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayers;
    public int maxJumpCount = 2;

    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public LayerMask wallLayers;

    private int jumpCount;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpCount = maxJumpCount;
    }

    void Update()
    {
        GroundCheck();
        Move();
        Jump();
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayers);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("AirSpeedY", rb.velocity.y);

        if (isGrounded && rb.velocity.y <= 0f)
            jumpCount = maxJumpCount;
    }

    void Move()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // ✅ 贴墙检测：防止空中推墙导致卡住
        bool touchingLeftWall = wallCheckLeft != null && Physics2D.OverlapCircle(wallCheckLeft.position, 0.1f, wallLayers);
        bool touchingRightWall = wallCheckRight != null && Physics2D.OverlapCircle(wallCheckRight.position, 0.1f, wallLayers);
        bool pushingWall =
            (!isGrounded) &&
            ((moveInput < 0 && touchingLeftWall) || (moveInput > 0 && touchingRightWall));

        if (!pushingWall)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        // ✅ 保留你原本的动画逻辑
        if (moveInput != 0)
        {
            animator.SetInteger("AnimState", 1);
            spriteRenderer.flipX = moveInput < 0;
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
            jumpCount--;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }

        if (wallCheckLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheckLeft.position, 0.1f);
        }

        if (wallCheckRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheckRight.position, 0.1f);
        }
    }
}
