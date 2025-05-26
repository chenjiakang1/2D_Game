using UnityEngine;
using System.Collections;

public class ChestOpener : MonoBehaviour
{
    private bool playerInRange = false;
    private bool isOpened = false;
    private Animator animator;

    [Header("宝箱设置")]
    public string playerTag = "Player";
    public string openTriggerName = "IsOpened";

    [Header("钥匙掉落设置")]
    public GameObject keyPrefab;                    // 拖入钥匙预制体
    public float dropDelay = 0.5f;                  // 开箱后延迟多久掉落钥匙
    public float pickupEnableDelay = 1.0f;          // 钥匙延迟多久可以拾取
    public Vector2 verticalOffsetRange = new Vector2(1f, 2f); // 生成位置Y偏移范围
    public Vector3 slideOffset = new Vector3(2f, -1f, 0f);  // 移动目标偏移
    public float slideDuration = 0.5f;              // 滑动到目标的动画时间

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && !isOpened && Input.GetKeyDown(KeyCode.F))
        {
            isOpened = true;
            animator.SetTrigger(openTriggerName);
            Debug.Log("🟢 Chest opening triggered.");

            Invoke(nameof(SpawnKey), dropDelay);
        }
    }

    void SpawnKey()
    {
        if (keyPrefab != null)
        {
            float yOffset = Random.Range(verticalOffsetRange.x, verticalOffsetRange.y);
            Vector3 spawnPos = transform.position + new Vector3(0, yOffset, 0);

            GameObject key = Instantiate(keyPrefab, spawnPos, Quaternion.identity);

            Collider2D col = key.GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = false; // 初始不可拾取
                StartCoroutine(EnableKeyPickup(col, pickupEnableDelay));
            }

            // ✅ 向右下滑动
            StartCoroutine(SlideKeyToTarget(key.transform, slideOffset, slideDuration));

            Debug.Log($"🔑 Key spawned and sliding from Y+{yOffset}.");
        }
        else
        {
            Debug.LogWarning("❗ Key prefab is not assigned.");
        }
    }

    private IEnumerator EnableKeyPickup(Collider2D collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.isTrigger = true;
        Debug.Log("✅ Key is now pickable.");
    }

    private IEnumerator SlideKeyToTarget(Transform keyTransform, Vector3 offset, float duration)
    {
        Vector3 startPos = keyTransform.position;
        Vector3 targetPos = startPos + offset;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            keyTransform.position = Vector3.Lerp(startPos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        keyTransform.position = targetPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
        }
    }
}
