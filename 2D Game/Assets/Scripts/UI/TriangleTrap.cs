using UnityEngine;

public class TriangleTrap : MonoBehaviour
{
    public float fallDistance = 8f;      // 掉落距离
    public float fallSpeed = 5f;         // 掉落速度
    public float shakeTime = 0.5f;       // 掉落前抖动时间
    public float shakeAmount = 0.1f;     // 抖动强度

    private Vector3 targetPosition;
    private bool isFalling = false;
    private bool isShaking = false;

    private Collider2D triggerCollider;
    private Collider2D killCollider;

    private Vector3 originalPosition;
    private Vector3 startPosition;
    private float shakeTimer;

    void Start()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            if (col.isTrigger)
                triggerCollider = col;
            else
                killCollider = col;
        }

        killCollider.enabled = false;  // 掉落杀人用Collider默认禁用

        // 记录初始位置（重置时使用）
        startPosition = transform.position;
        targetPosition = transform.position + Vector3.down * fallDistance;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTimer -= Time.deltaTime;

            Vector3 shakeOffset = new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0);
            transform.position = originalPosition + shakeOffset;

            if (shakeTimer <= 0f)
            {
                isShaking = false;
                isFalling = true;
            }
        }
        else if (isFalling)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

            if (!killCollider.enabled)
            {
                killCollider.enabled = true;
            }

            if (transform.position == targetPosition)
            {
                Destroy(gameObject); // 掉落完成，销毁
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFalling && !isShaking && other.CompareTag("Player"))
        {
            // 玩家进入触发 → 开始抖动
            isShaking = true;
            shakeTimer = shakeTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling && collision.collider.CompareTag("Player"))
        {
            // 砸到玩家 → 玩家死亡
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SendMessage("KillByTrap");
            }
        }
    }

    // 新增：重置陷阱
    public void ResetTrap()
    {
        transform.position = startPosition;
        isShaking = false;
        isFalling = false;
        killCollider.enabled = false;
        originalPosition = startPosition;
        targetPosition = startPosition + Vector3.down * fallDistance;
    }
}
