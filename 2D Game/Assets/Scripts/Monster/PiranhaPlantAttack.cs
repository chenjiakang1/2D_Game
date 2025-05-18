using UnityEngine;

public class PiranhaPlantAttack : MonoBehaviour
{
    [Header("攻击设置")]
    public float damageAmount = 10f; // 每次攻击造成的伤害

    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player != null)
        {
            FacePlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isPlayerNear", true);
            player = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isPlayerNear", false);
            player = null;
        }
    }

    // ✅ 正确的朝向判断（默认朝左）
    void FacePlayer()
    {
        float direction = player.position.x - transform.position.x;

        // 当前朝右时玩家在左 ➜ 翻转回来
        if (direction < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
        // 当前朝左时玩家在右 ➜ 翻转过去
        else if (direction > 0 && transform.localScale.x < 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    // ✅ 在动画事件中调用的攻击伤害逻辑
    public void ApplyDamage()
    {
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage(damageAmount);
            }
        }
    }
}
