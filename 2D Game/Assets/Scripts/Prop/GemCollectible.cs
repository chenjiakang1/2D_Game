using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    public AudioClip collectSound; // 吃钻石的音效

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 播放音效（从 Player 身上找 AudioSource）
            AudioSource playerAudio = other.GetComponent<AudioSource>();
            if (playerAudio != null && collectSound != null)
            {
                playerAudio.PlayOneShot(collectSound);
            }

            // 增加 UI 上的钻石数量
            FindObjectOfType<GemUIController>().AddGem(1);

            // 销毁当前钻石对象
            Destroy(gameObject);
        }
    }
}


