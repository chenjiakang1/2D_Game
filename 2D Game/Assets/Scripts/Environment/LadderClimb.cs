using UnityEngine;
using System.Collections;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 3f;
    private bool isOnLadder = false;
    private bool isClimbing = false;
    private float originalGravity;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerController playerController;

    private Coroutine stopAnimCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalGravity = rb.gravityScale;

        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (isOnLadder && Input.GetKeyDown(KeyCode.Q))
        {
            isClimbing = !isClimbing;
            animator.SetBool("isClimbing", isClimbing);

            if (isClimbing)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;

                if (playerController != null)
                    playerController.allowMovement = false;
            }
            else
            {
                rb.gravityScale = originalGravity;
                animator.speed = 1f;
                animator.SetFloat("ClimbSpeed", 0f);

                if (playerController != null)
                    playerController.allowMovement = true;
            }
        }

        if (isClimbing)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            // 攀爬状态允许左右移动
            rb.velocity = new Vector2(h * climbSpeed, v * climbSpeed);
            animator.SetFloat("ClimbSpeed", Mathf.Max(Mathf.Abs(v), Mathf.Abs(h)));
            if (Mathf.Abs(v) > 0.01f || Mathf.Abs(h) > 0.01f)
            {
                if (stopAnimCoroutine != null)
                {
                    StopCoroutine(stopAnimCoroutine);
                    stopAnimCoroutine = null;
                }
                animator.speed = 1f;
            }
            else
            {
                if (stopAnimCoroutine == null)
                {
                    stopAnimCoroutine = StartCoroutine(StopAnimationWithDelay(2f));
                }
            }
        }
    }

    IEnumerator StopAnimationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.speed = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            isClimbing = false;

            if (stopAnimCoroutine != null)
            {
                StopCoroutine(stopAnimCoroutine);
                stopAnimCoroutine = null;
            }

            rb.gravityScale = originalGravity;
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            animator.SetBool("isClimbing", false);
            animator.SetFloat("ClimbSpeed", 0f);
            animator.speed = 1f;

            if (playerController != null)
                playerController.allowMovement = true;
        }
    }
}
