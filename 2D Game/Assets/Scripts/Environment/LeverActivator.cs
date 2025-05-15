using UnityEngine;

public class LeverActivator : MonoBehaviour
{
    public GameObject platform;           // 要显形的平台
    public Sprite crankUpSprite;          // 拉杆激活后的图像

    private bool isPlayerNear = false;
    private bool activated = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (platform != null)
            platform.SetActive(false); // 初始隐藏平台

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerNear && !activated)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (platform != null)
                    platform.SetActive(true);

                if (spriteRenderer != null && crankUpSprite != null)
                    spriteRenderer.sprite = crankUpSprite;

                activated = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
