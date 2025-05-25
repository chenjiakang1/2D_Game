using UnityEngine;

public class BossAttackZone : MonoBehaviour
{
    public float damageAmount = 50f;
    public float damageCooldown = 1f;

    private float lastDamageTime = -Mathf.Infinity;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time - lastDamageTime > damageCooldown)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
                Debug.Log("[BossAttackZone] 命中玩家，造成伤害！");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            lastDamageTime = -Mathf.Infinity; // 退出后重置冷却时间
        }
    }
}
