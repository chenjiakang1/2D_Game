using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.hasKey = true;
                Debug.Log("🟡 Player picked up the key!");
                Destroy(gameObject); // 拾取后销毁钥匙
            }
        }
    }
}

