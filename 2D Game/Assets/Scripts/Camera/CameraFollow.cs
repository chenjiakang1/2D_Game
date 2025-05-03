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

        // ��ȡ Tilemap ������߽磨��Ȼ�������� X �ᣩ
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

            // ����������Ŀ��ӷ�Χ
            float camHeight = cam.orthographicSize * 2;
            float camWidth = camHeight * cam.aspect;

            // ���� X �� Clamp ����
            float minX = minWorld.x + camWidth / 2;
            float maxX = maxWorld.x - camWidth / 2;

            // **ǿ������ Y �᷶Χ�� [-1, 1]**
            float minY = -3;
            float maxY = 2;

            // ֻ�е��߽��㹻��ʱ�Ž��� Clamp��������������������ƶ�
            if (maxX > minX) smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);

            transform.position = smoothedPosition;
        }
    }
}


