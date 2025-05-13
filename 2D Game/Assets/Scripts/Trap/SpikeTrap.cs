using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 无论是哪个 PlayerController，都调用 KillByTrap 方法
            other.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
        }
    }
}
