using UnityEngine;

public class BossSimpleHealth : MonoBehaviour
{
    public float maxHealth = 300f;
    public float currentHealth;
    public HealthBarUI healthBarUI;

    private bool isDead = false;

    void Start()
    {
        ResetHealth(); // 初始时满血
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("[Boss] 已死亡");
        Destroy(gameObject); // 或者触发动画
    }

    // ✅ 添加此方法：恢复满血
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);
    }
}
