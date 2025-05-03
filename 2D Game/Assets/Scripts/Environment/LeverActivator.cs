using UnityEngine;
using TMPro;

public class LeverActivator : MonoBehaviour
{
    public GameObject pressKText;         // 提示文字对象
    public GameObject platform;           // 要显形的平台
    public Vector3 offset = new Vector3(0, 1.5f, 0); // 提示文字偏移
    public Sprite crankUpSprite;          // 拉杆激活后的图像

    private bool isPlayerNear = false;
    private RectTransform canvasRect;
    private RectTransform textRect;
    private bool activated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (pressKText != null)
        {
            pressKText.SetActive(false);
            textRect = pressKText.GetComponent<RectTransform>();
            canvasRect = pressKText.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        if (platform != null)
            platform.SetActive(false); // 初始隐藏平台

        // 获取 SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerNear && !activated)
        {
            UpdateTextPosition();

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (platform != null)
                    platform.SetActive(true);

                if (pressKText != null)
                    pressKText.SetActive(false);

                if (spriteRenderer != null && crankUpSprite != null)
                    spriteRenderer.sprite = crankUpSprite; // 更换图象

                activated = true;
            }
        }
    }

    private void UpdateTextPosition()
    {
        if (textRect == null || canvasRect == null)
            return;

        Vector3 worldPos = transform.position + offset;
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        Vector2 anchoredPos = new Vector2(
            (viewportPos.x - 0.5f) * canvasRect.sizeDelta.x,
            (viewportPos.y - 0.5f) * canvasRect.sizeDelta.y
        );

        textRect.anchoredPosition = anchoredPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            isPlayerNear = true;

            if (pressKText != null)
            {
                pressKText.SetActive(true);
                UpdateTextPosition();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            if (pressKText != null)
                pressKText.SetActive(false);
        }
    }
}
