using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    private bool isPlayerNear = false;       // 
    private bool checkpointSet = false;      // 

    void Update()
    {
        if (isPlayerNear && !checkpointSet && Input.GetKeyDown(KeyCode.E))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerHealth ph = playerObj.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.SetCheckpoint(transform.position);
                    checkpointSet = true;
                    Debug.Log("[Checkpoint] 存档设置成功！");
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
