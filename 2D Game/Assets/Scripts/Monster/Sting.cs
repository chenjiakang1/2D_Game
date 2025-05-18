using UnityEngine;

public class Sting : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 10f; // 造成的伤害量

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 调用玩家的 TakeDamage 方法
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SendMessage("TakeDamage", damage); // 或 player.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
