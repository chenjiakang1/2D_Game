using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �������ĸ� PlayerController�������� KillByTrap ����
            other.SendMessage("KillByTrap", SendMessageOptions.DontRequireReceiver);
        }
    }
}
