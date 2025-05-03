using UnityEngine;

public class PlatformAttachment : MonoBehaviour
{
    private Transform playerOnPlatform;
    private Vector3 lastPlatformPosition;

    void Start()
    {
        lastPlatformPosition = transform.position;
    }

    void Update()
    {
        if (playerOnPlatform != null)
        {
            Vector3 delta = transform.position - lastPlatformPosition;
            playerOnPlatform.position += delta;
        }

        lastPlatformPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerOnPlatform = collision.collider.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (collision.collider.transform == playerOnPlatform)
                playerOnPlatform = null;
        }
    }
}

