using UnityEngine;

public class DinoEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    public GameObject deathEffectPrefab; // 拖入 enemy-death prefab

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool movingRight = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 确保 Patrol 点不跟随 Dino 移动
        leftPoint.parent = null;
        rightPoint.parent = null;

        movingRight = false;
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);

        // 防止轻微 Y 抖动
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        sr.flipX = movingRight;

        // 巡逻边界判断
        if (movingRight && transform.position.x >= rightPoint.position.x - 0.05f)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x + 0.05f)
        {
            movingRight = true;
        }
    }

    //  玩家踩中我（由玩家调用）
    public void OnStomped()
    {
        // 播放死亡特效
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 通知最近的 DinoSpawner，我已经死了
        DinoSpawner spawner = FindClosestSpawner();
        if (spawner != null)
        {
            spawner.Clear();
        }

        Destroy(gameObject);
    }

    //  查找最近的 DinoSpawner
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
}
