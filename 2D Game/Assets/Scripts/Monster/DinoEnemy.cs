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

    [Header("生命值设置")]
    public int maxHealth = 3;
    private int currentHealth;

    public HealthBarUI healthBarUI;  // ✅（可选）用于显示敌人血条

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
        audioSource = gameObject.AddComponent<AudioSource>();

        leftPoint.parent = null;
        rightPoint.parent = null;

        movingRight = false;
        currentHealth = maxHealth;

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);

        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        sr.flipX = movingRight;

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
            float colliderHalfWidth = boxCollider.bounds.extents.x;
            float extraOffset = 0.05f;
            Vector2 horizontalOffset = new Vector2(movingRight ? (colliderHalfWidth + extraOffset) : -(colliderHalfWidth + extraOffset), 0f);
            Vector2 verticalOffset = new Vector2(0f, 0.2f);
            Vector2 checkPosition = (Vector2)transform.position + horizontalOffset + verticalOffset;

            Collider2D hit = Physics2D.OverlapBox(checkPosition, wallCheckBoxSize, 0f, groundLayer);
            if (hit != null && Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
                movingRight = !movingRight;
            }
        }
    }

    // ✅ 被主角攻击时调用
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " 受到攻击，当前血量：" + currentHealth);

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
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
