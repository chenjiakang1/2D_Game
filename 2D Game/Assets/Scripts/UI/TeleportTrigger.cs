using UnityEngine;
using System.Collections;

public class TeleportTrigger : MonoBehaviour
{
    [Tooltip("传送目标位置（一个空物体）")]
    public Transform teleportTarget;

    [Tooltip("是否只传送一次")]
    public bool oneTimeUse = false;

    [Tooltip("触发后传送的延迟时间（秒）")]
    public float delayTime = 0.3f;

    [Tooltip("传送时播放的音效")]
    public AudioClip teleportSound;

    private bool hasTeleported = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && teleportTarget != null && (!oneTimeUse || !hasTeleported))
        {
            StartCoroutine(DelayedTeleport(other.transform));
            hasTeleported = true;
        }
    }

    IEnumerator DelayedTeleport(Transform player)
    {
        //  播放传送音效
        if (teleportSound != null)
            AudioSource.PlayClipAtPoint(teleportSound, transform.position);

        yield return new WaitForSeconds(delayTime);

        player.position = teleportTarget.position;
    }
}
