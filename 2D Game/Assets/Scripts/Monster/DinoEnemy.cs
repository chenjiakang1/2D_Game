using UnityEngine;

public class DinoEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;

    [Header("墙体检测设置")]
    public Vector2 wallCheckBoxSize = new Vector2(0.1f, 0.5f);
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;
    private bool movingRight = false;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        leftPoint.parent = null;
        rightPoint.parent = null;

        movingRight = false;

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // 移动
        Vector2 velocity = rb.velocity;
        velocity.x = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        sr.flipX = movingRight;

        // ✅ 优先判断是否到达巡逻边界
        if (movingRight && transform.position.x >= rightPoint.position.x - 0.05f)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x + 0.05f)
        {
            movingRight = true;
        }
        else
        {
            // ✅ 没撞到边界，再检查前方是否有墙体
            float colliderHalfWidth = boxCollider.bounds.extents.x;
            float extraOffset = 0.05f;
            Vector2 horizontalOffset = new Vector2(movingRight ? (colliderHalfWidth + extraOffset) : -(colliderHalfWidth + extraOffset), 0f);
            Vector2 verticalOffset = new Vector2(0f, 0.2f); // 上移，避免检测地面
            Vector2 checkPosition = (Vector2)transform.position + horizontalOffset + verticalOffset;

            Collider2D hit = Physics2D.OverlapBox(checkPosition, wallCheckBoxSize, 0f, groundLayer);
            if (hit != null && Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                //Debug.Log("Wall detected by OverlapBox. Turning around.");
                rb.velocity = new Vector2(0f, rb.velocity.y);
                movingRight = !movingRight;
            }
        }
    }

    public void OnStomped()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        DinoSpawner spawner = FindClosestSpawner();
        if (spawner != null)
        {
            spawner.Clear();
        }

        Destroy(gameObject, deathSound != null ? deathSound.length : 0f);
    }

    private DinoSpawner FindClosestSpawner()
    {
        DinoSpawner[] spawners = FindObjectsOfType<DinoSpawner>();
        DinoSpawner closest = null;
        float minDist = Mathf.Infinity;

        foreach (var s in spawners)
        {
            float d = Vector2.Distance(transform.position, s.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = s;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) return;

        float colliderHalfWidth = Application.isPlaying ? boxCollider.bounds.extents.x : 0.5f;
        float extraOffset = 0.05f;
        Vector2 horizontalOffset = new Vector2(movingRight ? (colliderHalfWidth + extraOffset) : -(colliderHalfWidth + extraOffset), 0f);
        Vector2 verticalOffset = new Vector2(0f, 0.2f);
        Vector2 pos = (Vector2)transform.position + horizontalOffset + verticalOffset;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos, wallCheckBoxSize);
    }
}
