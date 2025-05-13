using UnityEngine;

public class TriangleTrap : MonoBehaviour
{
    public float fallDistance = 8f;
    public float fallSpeed = 5f;
    public float shakeTime = 0.5f;
    public float shakeAmount = 0.1f;

    public bool startHidden = false;   // 初始是否隐藏
    public bool rotateZ90 = false;     // 是否旋转Z轴 90度（水平模式）

    private Vector3 targetPosition;
    private bool isFalling = false;
    private bool isShaking = false;

    private Collider2D triggerCollider;
    private Collider2D killCollider;
    private SpriteRenderer spriteRenderer;

    private Vector3 originalPosition;
    private Vector3 startPosition;
    private float shakeTimer;

    void Start()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col.isTrigger)
                triggerCollider = col;
            else
                killCollider = col;
        }

        killCollider.enabled = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && startHidden)
        {
            spriteRenderer.enabled = false;
        }

        if (rotateZ90)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }

        startPosition = transform.position;

        if (rotateZ90)
            targetPosition = transform.position + Vector3.left * fallDistance;
        else
            targetPosition = transform.position + Vector3.down * fallDistance;

        originalPosition = transform.position;
    }

    void Update()
    {
        if (rotateZ90)
        {
            if (isFalling)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);
                if (!killCollider.enabled) killCollider.enabled = true;

                if (transform.position == targetPosition)
                {
                    FinishTrap(); //  模拟“销毁”状态
                }
            }
        }
        else
        {
            if (isShaking)
            {
                shakeTimer -= Time.deltaTime;
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-shakeAmount, shakeAmount),
                    Random.Range(-shakeAmount, shakeAmount),
                    0);
                transform.position = originalPosition + shakeOffset;

                if (shakeTimer <= 0f)
                {
                    isShaking = false;
                    isFalling = true;
                }
            }
            else if (isFalling)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);
                if (!killCollider.enabled) killCollider.enabled = true;

                if (transform.position == targetPosition)
                {
                    FinishTrap(); //  模拟“销毁”状态
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isFalling && !isShaking)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;

            if (rotateZ90)
            {
                isFalling = true;
            }
            else
            {
                isShaking = true;
                shakeTimer = shakeTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling && collision.collider.CompareTag("Player"))
        {
            collision.collider.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void FinishTrap()
    {
        isFalling = false;

        //  模拟“销毁”：禁用所有组件但保留对象以便 Reset
        if (killCollider != null) killCollider.enabled = false;
        if (triggerCollider != null) triggerCollider.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    public void ResetTrap()
    {
        transform.position = startPosition;
        isShaking = false;
        isFalling = false;

        if (triggerCollider != null) triggerCollider.enabled = true;
        if (killCollider != null) killCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = !startHidden;

        originalPosition = startPosition;

        if (rotateZ90)
            targetPosition = startPosition + Vector3.left * fallDistance;
        else
            targetPosition = startPosition + Vector3.down * fallDistance;
    }
}
