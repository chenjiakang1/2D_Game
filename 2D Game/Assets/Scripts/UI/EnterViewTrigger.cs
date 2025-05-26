using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterViewTrigger : MonoBehaviour
{
    public string targetSceneName = "NextScene"; // 要进入的关卡名
    public GameObject noKeyPanel; // ❗ 拖入提示面板

    private bool playerInRange = false;
    private GameObject currentPlayer;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            var inventory = currentPlayer?.GetComponent<PlayerInventory>();
            if (inventory != null && inventory.hasKey)
            {
                Debug.Log("🟢 Player has the key! Entering next scene...");
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.Log("🔒 You need a key to enter this area!");
                if (noKeyPanel != null)
                {
                    noKeyPanel.SetActive(true); // ✅ 显示提示面板
                    Invoke(nameof(HidePanel), 2f); // 2 秒后自动隐藏
                }
            }
        }
    }

    void HidePanel()
    {
        if (noKeyPanel != null)
            noKeyPanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
            Debug.Log("🟡 Player entered the portal area.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            currentPlayer = null;
            Debug.Log("🔵 Player left the portal area.");
        }
    }
}
