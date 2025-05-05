using UnityEngine;

public class TriangleTrap : MonoBehaviour
{
    public float fallDistance = 8f;      // �������
    public float fallSpeed = 5f;         // �����ٶ�
    public float shakeTime = 0.5f;       // ����ǰ����ʱ��
    public float shakeAmount = 0.1f;     // ����ǿ��

    private Vector3 targetPosition;
    private bool isFalling = false;
    private bool isShaking = false;

    private Collider2D triggerCollider;
    private Collider2D killCollider;

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

        killCollider.enabled = false;  // ����ɱ����ColliderĬ�Ͻ���

        // ��¼��ʼλ�ã�����ʱʹ�ã�
        startPosition = transform.position;
        targetPosition = transform.position + Vector3.down * fallDistance;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTimer -= Time.deltaTime;

            Vector3 shakeOffset = new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0);
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

            if (!killCollider.enabled)
            {
                killCollider.enabled = true;
            }

            if (transform.position == targetPosition)
            {
                Destroy(gameObject); // ������ɣ�����
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFalling && !isShaking && other.CompareTag("Player"))
        {
            // ��ҽ��봥�� �� ��ʼ����
            isShaking = true;
            shakeTimer = shakeTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFalling && collision.collider.CompareTag("Player"))
        {
            // �ҵ���� �� �������
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SendMessage("KillByTrap");
            }
        }
    }

    // ��������������
    public void ResetTrap()
    {
        transform.position = startPosition;
        isShaking = false;
        isFalling = false;
        killCollider.enabled = false;
        originalPosition = startPosition;
        targetPosition = startPosition + Vector3.down * fallDistance;
    }
}
