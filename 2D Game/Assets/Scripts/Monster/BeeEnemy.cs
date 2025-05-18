using UnityEngine;
using System.Collections;

public class BeeEnemy : MonoBehaviour
{
    [Header("飞行与巡逻")]
    public float speed = 2f;
    public float flyHeight = 2f;
    public float patrolRadius = 0.2f;
    public float maxOffsetX = 3f;
    public float minY = 0f;
    public float maxY = 10f;
    public float bounceCooldown = 0.3f;
    public LayerMask groundLayer;

    [Header("玩家检测与毒刺攻击")]
    public float detectionRange = 5f;
    public GameObject stingPrefab;
    public Transform firePoint;
    public float stingSpeed = 5f;
    public float attackCooldown = 2f;
    public float stopApproachDistance = 2.5f;
    public float attackApproachSpeed = 1.5f;

    [Header("生命与死亡")]
    public float maxHealth = 3f;
    private float currentHealth;
    public HealthBarUI healthBarUI;
    public GameObject deathEffectPrefab;
    public float respawnDelay = 3f;

    [Header("音效")]
    public AudioClip hurtClip;
    private AudioSource audioSource;

    private Vector3 startPos;
    private bool movingRight = true;
    private float floatOffset;
    private float lastBounceTime = -999f;
    private float lastAttackTime = -999f;

    private GameObject player;
    private bool playerDetected = false;

    void Start()
    {
        startPos = transform.position;
        floatOffset = Random.Range(0f, 2f * Mathf.PI);
        player = GameObject.FindGameObjectWithTag("Player");

        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);
    }

    void Update()
    {
        if (player == null) return;

        float currentTime = Time.time;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!playerDetected)
                playerDetected = true;

            bool shouldFaceRight = player.transform.position.x > transform.position.x;
            if (shouldFaceRight != movingRight)
            {
                movingRight = shouldFaceRight;
                Flip();
            }

            ApproachAndAttack(currentTime, distanceToPlayer);
            return;
        }

        if (playerDetected)
        {
            playerDetected = false;
        }

        Patrol(currentTime);
    }

    void Patrol(float currentTime)
    {
        float sinY = Mathf.Sin(currentTime * 2f + floatOffset) * flyHeight * 0.5f;
        float newY = startPos.y + sinY;

        float moveDir = movingRight ? 1f : -1f;
        Vector2 checkPos = transform.position + new Vector3(moveDir * patrolRadius, 0f);
        bool hitWall = Physics2D.OverlapCircle(checkPos, patrolRadius, groundLayer);

        float offsetX = Mathf.Abs(transform.position.x - startPos.x);
        bool outOfBounds = offsetX > maxOffsetX || newY < minY || newY > maxY;

        if ((hitWall || outOfBounds) && currentTime - lastBounceTime > bounceCooldown)
        {
            movingRight = !movingRight;
            Flip();
            lastBounceTime = currentTime;
        }

        Vector3 move = new Vector3((movingRight ? 1f : -1f) * speed * Time.deltaTime, 0f, 0f);
        transform.position += move;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void ApproachAndAttack(float time, float distanceToPlayer)
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (distanceToPlayer > stopApproachDistance)
        {
            Vector3 move = new Vector3(direction.x * attackApproachSpeed * Time.deltaTime, 0f, 0f);
            transform.position += move;
        }

        if (Time.time - lastAttackTime > attackCooldown)
        {
            ShootStinger();
            lastAttackTime = Time.time;
        }
    }

    void ShootStinger()
    {
        if (stingPrefab == null || firePoint == null) return;

        GameObject sting = Instantiate(stingPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = sting.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (player.transform.position - firePoint.position).normalized;
            rb.velocity = dir * stingSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hurtClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hurtClip); // ✅ 播放受击音效
        }

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
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

        DinoSpawner spawner = FindObjectOfType<DinoSpawner>();
        if (spawner != null)
        {
            spawner.Clear();
            StartCoroutine(RespawnFromSpawner(spawner));
        }

        Destroy(gameObject);
    }

    IEnumerator RespawnFromSpawner(DinoSpawner spawner)
    {
        yield return new WaitForSeconds(respawnDelay);
        spawner.Spawn();
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (movingRight ? -1f : 1f);
        transform.localScale = scale;

        if (healthBarUI != null)
        {
            Transform bar = healthBarUI.transform;
            Vector3 barScale = bar.localScale;
            barScale.x = Mathf.Abs(barScale.x);
            bar.localScale = barScale;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 checkPos = transform.position + new Vector3(movingRight ? patrolRadius : -patrolRadius, 0f);
        Gizmos.DrawWireSphere(checkPos, patrolRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
