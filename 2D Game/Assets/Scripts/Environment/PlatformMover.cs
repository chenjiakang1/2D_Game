using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right; // �ƶ�����
    public float moveDistance = 10f;              // �ƶ����루10��λ��
    public float moveSpeed = 2f;                  // �ƶ��ٶ�

    private Vector3 startPoint;
    private Vector3 targetPoint;
    private bool movingToTarget = true;

    void Start()
    {
        startPoint = transform.position;
        targetPoint = startPoint + (Vector3)(moveDirection.normalized * moveDistance);
    }

    void Update()
    {
        Vector3 destination = movingToTarget ? targetPoint : startPoint;

        // ƽ���ƶ�
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // ����Ŀ������
        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            movingToTarget = !movingToTarget;
        }
    }
}

