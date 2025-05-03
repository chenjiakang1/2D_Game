using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right; // 移动方向
    public float moveDistance = 10f;              // 移动距离（10单位）
    public float moveSpeed = 2f;                  // 移动速度

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

        // 平滑移动
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // 到达目标点后反向
        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            movingToTarget = !movingToTarget;
        }
    }
}

