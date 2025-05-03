using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    public AudioClip collectSound; // ����ʯ����Ч

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ������Ч���� Player ������ AudioSource��
            AudioSource playerAudio = other.GetComponent<AudioSource>();
            if (playerAudio != null && collectSound != null)
            {
                playerAudio.PlayOneShot(collectSound);
            }

            // ���� UI �ϵ���ʯ����
            FindObjectOfType<GemUIController>().AddGem(1);

            // ���ٵ�ǰ��ʯ����
            Destroy(gameObject);
        }
    }
}


