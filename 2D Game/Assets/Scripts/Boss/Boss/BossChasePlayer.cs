using UnityEngine;
using System.Collections;

public class BossChasePlayer : MonoBehaviour
{
    [Header("基础设置")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.2f;

    [Header("攻击设置")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float attack1Damage = 50f;
    public float attack2Damage = 30f;

    [Header("能量球设定")]
    public GameObject boltPrefab;
    public float boltCheckInterval = 5f;
    private float boltTimer;

    private float lastAttackTime;
    private bool isAttacking = false;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer backEffect;

    private bool isFacingRight = true;

    [Header("攻击音效")]
    public AudioClip attack1Sound;
    public AudioClip attack2Sound;
    public AudioClip boltCastSound;

    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            Debug.LogWarning("未找到玩家对象，请确认 Player 标签是否正确设置");
        }

        spriteRenderer.flipX = false;
        isFacingRight = true;

        Transform backTransform = transform.Find("Back");
        if (backTransform != null)
        {
            backEffect = backTransform.GetComponent<SpriteRenderer>();
            if (backEffect != null)
                backEffect.color = new Color(1, 1, 1, 0);
        }
    }

    void FixedUpdate()
    {
        if (player == null || isAttacking) return;

        Vector2 bossPos = transform.position;
        Rect chaseRect = new Rect(bossPos.x - 10f, bossPos.y - 2f, 20f, 3f);
        bool inChaseRect = chaseRect.Contains(player.position);

        Debug.Log($"[Boss] 玩家位置：{player.position}，追击区域中心：{bossPos}，是否在矩形范围：{inChaseRect}");

        if (inChaseRect)
        {
            float distance = Vector2.Distance(bossPos, player.position);
            if (distance > stopDistance)
            {
                animator.SetBool("isWalking", true);
                MoveTowardsPlayer();
                Debug.Log("[Boss] 开始追击玩家");
            }
            else
            {
                animator.SetBool("isWalking", false);
                TryAttackPlayer();
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            Debug.Log("[Boss] 玩家超出追击范围，停止追击");
        }

        boltTimer += Time.fixedDeltaTime;
        if (boltTimer >= boltCheckInterval)
        {
            TryCastBolt();
            boltTimer = 0f;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        float xDistance = player.position.x - transform.position.x;

        if (xDistance < -1f && isFacingRight)
        {
            spriteRenderer.flipX = true; // 修复转向反了
            isFacingRight = false;
            Debug.Log("[Boss] 玩家在左侧，转向左");
        }
        else if (xDistance > 1f && !isFacingRight)
        {
            spriteRenderer.flipX = false; // 修复转向反了
            isFacingRight = true;
            Debug.Log("[Boss] 玩家在右侧，转向右");
        }

        Vector2 targetPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
        Debug.Log($"[Boss] 移动方向：{direction.x:F2}，目标位置：{targetPosition}");
    }

    void TryAttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(PerformAttackWithDelay());
            lastAttackTime = Time.time;
        }
    }

    IEnumerator PerformAttackWithDelay()
    {
        isAttacking = true;

        PlayerHealth playerHealth = player?.GetComponent<PlayerHealth>();
        if (playerHealth == null) yield break;

        int attackType = Random.Range(0, 2);

        if (attackType == 0)
        {
            animator.SetTrigger("Attack1");

            if (audioSource != null && attack1Sound != null)
                audioSource.PlayOneShot(attack1Sound);

            yield return null;
            playerHealth.TakeDamage(attack1Damage);
            Debug.Log($"[Boss] 使用攻击1，造成 {attack1Damage} 点伤害");
        }
        else
        {
            animator.SetTrigger("Attack2");
            Debug.Log("[Boss] 使用攻击2，等待动画事件");
        }

        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    public void ApplyAttack2Damage()
    {
        if (player == null || !isAttacking) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > attackRange) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attack2Damage);
            Debug.Log($"[Boss] 攻击2 动画事件触发，造成 {attack2Damage} 点伤害");

            if (audioSource != null && attack2Sound != null)
                audioSource.PlayOneShot(attack2Sound);
        }
    }

    void TryCastBolt()
    {
        if (player == null || boltPrefab == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float playerHeight = player.position.y;

        if ((distance < 10f || distance > 15f) && playerHeight <= 2.5f) return;

        Debug.Log("[Boss] 触发能量球释放");
        if (audioSource != null && boltCastSound != null)
            audioSource.PlayOneShot(boltCastSound);

        StartCoroutine(FadeBackEffect());

        Vector3 facingDir = isFacingRight ? Vector3.right : Vector3.left;
        Vector3 basePos = transform.position + facingDir * 1.5f;

        Instantiate(boltPrefab, basePos + Vector3.up * 0.5f, Quaternion.identity);
        Instantiate(boltPrefab, basePos, Quaternion.identity);
        Instantiate(boltPrefab, basePos + Vector3.down * 0.5f, Quaternion.identity);
    }

    IEnumerator FadeBackEffect()
    {
        if (backEffect == null) yield break;

        float duration = 0.2f;
        float holdTime = 0.3f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / duration);
            backEffect.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        backEffect.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(holdTime);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / duration);
            backEffect.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        backEffect.color = new Color(1, 1, 1, 0);
    }

    // ✅ 可视化矩形追击范围
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(20f, 4f, 0f); // 左右10，高度3
        Gizmos.DrawWireCube(center, size);
    }
}
