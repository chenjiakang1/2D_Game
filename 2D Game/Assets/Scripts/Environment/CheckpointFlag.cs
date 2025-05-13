using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    private bool isPlayerNear = false;       // ����Ƿ��ڷ�Χ��
    private bool checkpointSet = false;      // �Ƿ��Ѿ����ù�

    void Update()
    {
        // �������ڷ�Χ�ڣ���δ���ù��浵�����Ұ��� E ��
        if (isPlayerNear && !checkpointSet && Input.GetKeyDown(KeyCode.E))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerController player = playerObj.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.SetCheckpoint(transform.position);  // ������ҵĸ������º���
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
