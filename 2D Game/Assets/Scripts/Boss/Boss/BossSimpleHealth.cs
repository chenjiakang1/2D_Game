using UnityEngine;

public class BossSimpleHealth : MonoBehaviour
{
    public float maxHealth = 300f;
    public float currentHealth;
    public HealthBarUI healthBarUI;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth, maxHealth);
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
}

