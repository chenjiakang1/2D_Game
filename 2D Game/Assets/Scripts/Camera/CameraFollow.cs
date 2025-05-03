using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public Tilemap tilemap;

    private Camera cam;
    private Vector3 minWorld, maxWorld;

    void Start()
    {
        cam = Camera.main;

        // 获取 Tilemap 的世界边界（仍然保留用于 X 轴）
        BoundsInt bounds = tilemap.cellBounds;
        minWorld = tilemap.CellToWorld(bounds.min);
        maxWorld = tilemap.CellToWorld(bounds.max);

        Debug.Log($"Tilemap Bounds: Min({minWorld}), Max({maxWorld})");
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // 计算摄像机的可视范围
            float camHeight = cam.orthographicSize * 2;
            float camWidth = camHeight * cam.aspect;

            // 修正 X 轴 Clamp 限制
            float minX = minWorld.x + camWidth / 2;
            float maxX = maxWorld.x - camWidth / 2;

            // **强制限制 Y 轴范围在 [-1, 1]**
            float minY = -3;
            float maxY = 2;

            // 只有当边界足够大时才进行 Clamp，否则允许摄像机自由移动
            if (maxX > minX) smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);

            transform.position = smoothedPosition;
        }
    }
}


