using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool allowMovement = true;
    public bool allowFallDeath = true;
    public float fallDeathY = -7f;
    public Sprite idleSprite;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerRoll roll;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        roll = GetComponent<PlayerRoll>();
    }

    void Update()
    {
        if (allowFallDeath && transform.position.y < fallDeathY)
            GetComponent<PlayerHealth>()?.KillByTrap();

        if (Input.GetKeyDown(KeyCode.R))
            GetComponent<PlayerHealth>()?.Respawn();
    }
}
