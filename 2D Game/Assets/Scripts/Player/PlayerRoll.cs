using UnityEngine;
using System.Collections;

public class PlayerRoll : MonoBehaviour
{
    public float rollDistance = 1.5f;
    public float rollDuration = 0.3f;
    public LayerMask groundLayers;

    private bool isRolling = false;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public bool IsRolling => isRolling;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = animator.GetBool("Grounded");
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
        Vector2 origin = rb.position;
        Vector2 targetPos = origin + new Vector2(direction * rollDistance, 0);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * direction, rollDistance, groundLayers);
        if (hit.collider != null)
            targetPos = origin + new Vector2(direction * hit.distance, 0);

        float time = 0f;
        while (time < rollDuration)
        {
            Vector2 newPos = Vector2.Lerp(origin, targetPos, time / rollDuration);
            rb.MovePosition(newPos);
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.MovePosition(targetPos);
        isRolling = false;
    }
}
