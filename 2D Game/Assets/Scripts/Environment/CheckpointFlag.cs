using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    private bool isPlayerNear = false;       // 玩家是否在范围内
    private bool checkpointSet = false;      // 是否已经设置过

    void Update()
    {
        // 如果玩家在范围内，尚未设置过存档，并且按下 E 键
        if (isPlayerNear && !checkpointSet && Input.GetKeyDown(KeyCode.E))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerController player = playerObj.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.SetCheckpoint(transform.position);  // 调用玩家的复活点更新函数
                    checkpointSet = true;
                    Debug.Log(" Checkpoint saved at: " + transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!checkpointSet && other.CompareTag("Player"))
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
