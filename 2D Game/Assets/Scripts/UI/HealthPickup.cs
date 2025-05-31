using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 30f;
    public AudioClip healSound;                //  治疗音效
    private static AudioSource audioSource;    //  静态共享播放器

    void Start()
    {
        // 找场景中唯一的 AudioSource（或你可以专门挂一个空对象来播放）
        if (audioSource == null)
        {
            GameObject obj = new GameObject("HealthPickupAudio");
            audioSource = obj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                //  播放治疗音效
                if (healSound != null)
                {
                    audioSource.PlayOneShot(healSound);
                }

                Destroy(gameObject); // 吃掉血包
            }
        }
    }
}
