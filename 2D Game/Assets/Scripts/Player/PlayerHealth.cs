using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public AudioClip deathSound;
    public AudioClip hurtSound;
    public HealthBarUI healthBarUI;

    private float currentHealth;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private Collider2D playerCollider;
    private Vector3 checkpointPosition;
    private PlayerBlock block;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();
        block = GetComponent<PlayerBlock>();
        checkpointPosition = transform.position;
        currentHealth = maxHealth;

        healthBarUI?.SetHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (block != null && block.IsBlocking)
        {
            block.PlayBlockSound();
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthBarUI?.SetHealth(currentHealth, maxHealth);

        if (hurtSound != null && audioSource != null)
            audioSource.PlayOneShot(hurtSound);

        if (currentHealth <= 0f)
            StartCoroutine(Die());
    }

    public void KillByTrap()
    {
        if (!isDead)
            StartCoroutine(Die());
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
    }

    IEnumerator Die()
    {
        isDead = true;

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        animator.SetTrigger("Hurt");
        if (playerCollider != null) playerCollider.enabled = false;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        animator.SetInteger("AnimState", 0);
        Respawn();
    }

    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        healthBarUI?.SetHealth(currentHealth, maxHealth);

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        transform.position = checkpointPosition + Vector3.up * 0.5f;

        animator.ResetTrigger("Hurt");
        animator.Play("Idle");

        foreach (var s in FindObjectsOfType<DinoSpawner>()) s.Spawn();
        foreach (var trap in FindObjectsOfType<TriangleTrap>()) trap.ResetTrap();

        StartCoroutine(WaitThenRevive());
    }

    IEnumerator WaitThenRevive()
    {
        yield return new WaitForSeconds(0.2f);
        rb.gravityScale = 1.5f;
        if (playerCollider != null) playerCollider.enabled = true;
    }
}
