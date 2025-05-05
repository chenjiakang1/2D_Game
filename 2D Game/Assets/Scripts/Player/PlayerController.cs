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

    public AudioClip deathSound;
    private AudioSource audioSource;

    // ˫�������
    private int jumpCount;
    public int maxJumpCount = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;

        isDead = false;
        animator.SetBool("isIdle", true);
        animator.ResetTrigger("Hurt");
    }

    void Update()
    {
        if (!isDead && allowMovement)
        {
            Move();
            Jump();
        }

        // ����ȥ�ж�
        if (transform.position.y < -7f && !isDead)
        {
            StartCoroutine(Die());
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // ���������Ծ����
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

            animator.SetTrigger("Jump");

            jumpCount--;
        }
    }

    // �������ͳһ����
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

    // ����
    void Respawn()
    {
        Debug.Log("Respawning at starting position.");
        isDead = false;

        Vector3 respawnPosition = new Vector3(-7.27f, -0.35f, 0f); // �̶���ʼλ�ã��ɸ�Ϊ�浵�㣩
        transform.position = respawnPosition + Vector3.up * 0.5f;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 1.5f;

        DinoSpawner[] spawners = FindObjectsOfType<DinoSpawner>();
        foreach (var s in spawners)
        {
            s.Spawn();
        }

        // �ָ����� TriangleTrap
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

    // �������ײ���
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDead && collision.collider.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y >= 0.5f)
                {
                    // �ȵ���
                    DinoEnemy dino = collision.collider.GetComponent<DinoEnemy>();
                    if (dino != null)
                    {
                        dino.OnStomped();
                    }

                    rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f);
                    jumpCount = maxJumpCount - 1;

                    return;
                }
            }

            // ���ִ���
            StartCoroutine(Die());
        }
    }

    // ����ɱ����TriangleTrap ���������
    public void KillByTrap()
    {
        if (!isDead)
        {
            StartCoroutine(Die());
        }
    }

    // ����������򣨵����ã�
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
