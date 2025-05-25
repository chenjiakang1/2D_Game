using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 5f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;
    public AudioClip attackSound1;

    private float attackCooldown = 0.2f;
    private float attackTimer;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    public int bossAttackDamage = 25; // 👉 Boss 每次受到 25 点伤害

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        animator.SetTrigger("Attack");
        if (attackSound1 != null) audioSource.PlayOneShot(attackSound1);

        float dir = spriteRenderer.flipX ? -1f : 1f;
        attackPoint.position = transform.position + new Vector3(dir * 1f, 0f, 0f);

        foreach (var enemy in Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers))
        {
            enemy.GetComponent<DinoEnemy>()?.TakeDamage(attackDamage);
            enemy.GetComponent<BeeEnemy>()?.TakeDamage(1f);
            enemy.GetComponent<BossSimpleHealth>()?.TakeDamage(bossAttackDamage); // ✅ 单独伤害
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
