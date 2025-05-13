using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Tilemap tilemap;

    public float minY = -3f;  //  可配置的 Y 最小值
    public float maxY = 2f;   //  可配置的 Y 最大值

    private Camera cam;
    private Vector3 minWorld, maxWorld;

    void Start()
    {
        cam = Camera.main;

        if (tilemap == null)
        {
            Debug.LogError("Tilemap 未设置，请检查 CameraFollow 脚本的 tilemap 引用！");
            return;
        }

        // 获取 Tilemap 的世界边界（用于限制 X 轴）
        BoundsInt bounds = tilemap.cellBounds;
        minWorld = tilemap.CellToWorld(bounds.min);
        maxWorld = tilemap.CellToWorld(bounds.max);

        Debug.Log($"Tilemap Bounds: Min({minWorld}), Max({maxWorld})");
    }

    void LateUpdate()
    {
        if (target != null && cam != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            float camHeight = cam.orthographicSize * 2;
            float camWidth = camHeight * cam.aspect;

            // 限制 X
            float minX = minWorld.x + camWidth / 2;
            float maxX = maxWorld.x - camWidth / 2;

            if (maxX > minX)
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);

            // 限制 Y 使用变量
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);

            transform.position = smoothedPosition;
        }
    }
}
