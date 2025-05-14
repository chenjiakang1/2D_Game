using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float jumpForce = 7.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private Vector3 originalScale;

    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isDead = false;

    public bool allowMovement = true;
    public bool allowFallDeath = true;

    public AudioClip deathSound;
    private AudioSource audioSource;

    private int jumpCount;
    public int maxJumpCount = 2;

    private Vector3 checkpointPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;

        isDead = false;
        animator.SetBool("isIdle", true);
        animator.ResetTrigger("Hurt");

        checkpointPosition = transform.position;
    }

    void Update()
    {
        if (!isDead && allowMovement)
        {
            Move();
            Jump();
        }

        if (allowFallDeath && transform.position.y < -7f && !isDead)
        {
            StartCoroutine(Die());
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        animator.SetBool("isGrounded", isGrounded);  // ✅ 同步给 Animator

        if (isGrounded && rb.velocity.y <= 0f)
        {
            jumpCount = maxJumpCount;
        }
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        animator.SetBool("isRunning", moveInput != 0);

        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * originalScale.x, originalScale.y, originalScale.z);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //animator.SetTrigger("Jump");  // ✅ 使用 Trigger 触发跳跃动画
            jumpCount--;
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        Debug.Log("Player has died!");

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        animator.SetTrigger("Hurt");

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null) playerCollider.enabled = false;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        float hurtAnimTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(hurtAnimTime);

        animator.SetBool("isIdle", true);
        Respawn();
    }

    void Respawn()
    {
        Debug.Log("Respawning at checkpoint.");
        isDead = false;

        transform.position = checkpointPosition + Vector3.up * 0.5f;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 1.5f;

        DinoSpawner[] spawners = FindObjectsOfType<DinoSpawner>();
        foreach (var s in spawners)
        {
            s.Spawn();
        }

        TriangleTrap[] traps = FindObjectsOfType<TriangleTrap>();
        foreach (var trap in traps)
        {
            trap.ResetTrap();
        }

        StartCoroutine(EnableCollider());
    }

    IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.2f);
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null) playerCollider.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && collision.collider.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y >= 0.5f)
                {
                    DinoEnemy dino = collision.collider.GetComponent<DinoEnemy>();
                    if (dino != null) dino.OnStomped();

                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f);
                    jumpCount = maxJumpCount - 1;
                    return;
                }
            }

            StartCoroutine(Die());
        }
    }

    public void KillByTrap()
    {
        if (!isDead)
        {
            StartCoroutine(Die());
        }
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
        Debug.Log("Checkpoint updated: " + checkpointPosition);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
