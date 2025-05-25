using UnityEngine;

public class MagicBolt : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 20f;

    private Transform target;

    void Start()
    {
        // 寻找玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (target == null) return;

        // 朝玩家方向移动
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // ✅ 修正转向翻转逻辑（反转判断）
        if (direction.x > 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
        else if (direction.x < 0 && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("[MagicBolt] 撞击玩家造成伤害：" + damage);
            }

            Destroy(gameObject); // 撞击后销毁自身
        }
    }
}
