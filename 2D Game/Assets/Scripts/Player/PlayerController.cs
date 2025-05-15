using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayers;

    public AudioClip deathSound;
    public bool allowMovement = true;
    public bool allowFallDeath = true;

    public Sensor_HeroKnight sensorWallLeftTop;
    public Sensor_HeroKnight sensorWallLeftBottom;
    public Sensor_HeroKnight sensorWallRightTop;
    public Sensor_HeroKnight sensorWallRightBottom;

    public AudioClip attackSound1;
    public AudioClip blockSound;

    public float rollDistance = 1.5f;
    public float rollDuration = 0.3f;

    public Transform attackPoint;
    public float attackRange = 3f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;

    public HealthBarUI healthBarUI;

    private float maxHealth = 100f;
    private float currentHealth;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private bool isGrounded;
    private bool isDead = false;
    private float attackCooldown = 0.2f;
    private float attackTimer;
    private bool isRolling = false;
    private Vector3 checkpointPosition;

    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isWallSliding;

    private bool isBlocking = false;
    private bool isIdleBlocking = false;

    private float damageCooldown = 1f;
    private float damageTimer = 0f;

    private int maxJumpCount = 2;
    private int jumpCount;

    public Sprite idleSprite;

    private float blockSoundCooldown = 1f;
    private float blockSoundTimer = 0f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        checkpointPosition = transform.position;

        currentHealth = maxHealth;
        healthBarUI.SetHealth(currentHealth, maxHealth);
        jumpCount = maxJumpCount;
    }

    void Update()
    {
        damageTimer -= Time.deltaTime;
        blockSoundTimer -= Time.deltaTime;

        GroundCheck();
        WallSlideCheck();

        if (!isDead && allowMovement && !isRolling)
        {
            Move();
            Jump();
            Attack();
            Roll();
            Block();
        }

        if (allowFallDeath && transform.position.y < -7f && !isDead)
        {
            StartCoroutine(Die());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        animator.SetBool("Block", isBlocking);
        animator.SetBool("IdleBlock", isIdleBlocking);
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayers);
        animator.SetBool("Grounded", isGrounded);

        if (isGrounded && rb.velocity.y <= 0f)
        {
            jumpCount = maxJumpCount;
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayers);
        animator.SetBool("Grounded", isGrounded);
        animator.SetFloat("AirSpeedY", rb.velocity.y);
    }

    void WallSlideCheck()
    {
    bool wasWallSliding = isWallSliding;

    // 检测墙体传感器（你四个传感器都接入了）
    isTouchingWallLeft = sensorWallLeftTop.State() && sensorWallLeftBottom.State();
    isTouchingWallRight = sensorWallRightTop.State() && sensorWallRightBottom.State();

    // ✅ 滑墙状态：只要贴墙且不在地面
    isWallSliding = !isGrounded && (isTouchingWallLeft || isTouchingWallRight);

    animator.SetBool("WallSlide", isWallSliding);

    // ✅ 真正脱离滑墙：从滑墙中出来，且传感器都没贴墙
        if (wasWallSliding && !isWallSliding)
    {
    animator.enabled = false; // 临时禁用
    spriteRenderer.sprite = idleSprite; // 显示 Idle 第1帧的 Sprite
    animator.enabled = true;

    animator.Play("Idle", 0, 0f);     // 切动画
    animator.Update(0f);              // 刷新帧
    animator.SetInteger("AnimState", 0);
    }
    }

    void Move()
    {
        if (isBlocking) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput != 0)
        {
            animator.SetInteger("AnimState", 1);
            spriteRenderer.flipX = moveInput < 0;
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
            jumpCount--;
        }
    }

    void Attack()
{
    if (isBlocking) return;

    attackTimer -= Time.deltaTime;

    if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
    {
        animator.SetTrigger("Attack");

        if (attackSound1 != null)
            audioSource.PlayOneShot(attackSound1);

        // ✅ 动态设置攻击点到角色前方 1 单位
        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector3 attackOffset = new Vector3(direction * 1f, 0f, 0f); // 可调偏移量
        attackPoint.position = transform.position + attackOffset;

        // ✅ 发起攻击检测
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<DinoEnemy>()?.TakeDamage(attackDamage);
        }

        attackTimer = attackCooldown;
    }
}
void OnDrawGizmosSelected()
{
    if (attackPoint == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
}

    void Roll()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isRolling)
        {
            StartCoroutine(DoRoll());
        }
    }

    IEnumerator DoRoll()
    {
        isRolling = true;
        animator.SetTrigger("Roll");

        float direction = spriteRenderer.flipX ? -1f : 1f;
        Vector2 origin = transform.position;
        Vector2 targetPos = origin + new Vector2(direction * rollDistance, 0);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rollDistance, groundLayers);
        if (hit.collider != null)
        {
            float distanceToWall = hit.distance;
            targetPos = origin + new Vector2(direction * distanceToWall, 0);
        }

        float time = 0f;
        while (time < rollDuration)
        {
            float t = time / rollDuration;
            transform.position = Vector2.Lerp(origin, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isRolling = false;
    }

    void Block()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isBlocking = true;
            isIdleBlocking = true;
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            isBlocking = false;
            isIdleBlocking = false;
        }
    }

    void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthBarUI.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            StartCoroutine(Die());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    if (!isDead && collision.collider.CompareTag("Enemy") && damageTimer <= 0f)
    {
        if (isBlocking)
{
    if (blockSound != null && audioSource != null && blockSoundTimer <= 0f)
    {
        audioSource.PlayOneShot(blockSound);
        blockSoundTimer = blockSoundCooldown;
    }
    return;
}

        TakeDamage(maxHealth * 0.1f);
        damageTimer = damageCooldown;
    }
}



    void OnCollisionStay2D(Collision2D collision)
{
    if (!isDead && collision.collider.CompareTag("Enemy") && damageTimer <= 0f)
    {
        if (isBlocking)
{
    if (blockSound != null && audioSource != null && blockSoundTimer <= 0f)
    {
        audioSource.PlayOneShot(blockSound);
        blockSoundTimer = blockSoundCooldown;
    }
    return;
}

        TakeDamage(maxHealth * 0.1f);
        damageTimer = damageCooldown;
    }
}


    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
    }

    public void KillByTrap()
    {
        if (!isDead)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        animator.SetTrigger("Hurt");

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null) playerCollider.enabled = false;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        float hurtAnimTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(hurtAnimTime);

        animator.SetInteger("AnimState", 0);
        Respawn();
    }

    public void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;
        healthBarUI.SetHealth(currentHealth, maxHealth);

        transform.position = checkpointPosition + Vector3.up * 0.5f;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

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

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null) playerCollider.enabled = true;
    }
}
