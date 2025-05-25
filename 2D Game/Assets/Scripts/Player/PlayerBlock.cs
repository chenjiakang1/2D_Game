using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    public AudioClip blockSound;
    public float blockSoundCooldown = 1f;

    public bool IsBlocking => isBlocking;
    public bool IsIdleBlocking => isIdleBlocking;

    private bool isBlocking = false;
    private bool isIdleBlocking = false;

    private Animator animator;
    private AudioSource audioSource;
    private float blockSoundTimer;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        blockSoundTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K))
        {
            isBlocking = true;
            isIdleBlocking = true;
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            isBlocking = false;
            isIdleBlocking = false;
        }

        animator.SetBool("Block", isBlocking);
        animator.SetBool("IdleBlock", isIdleBlocking);
    }

    public void PlayBlockSound()
    {
        if (blockSound != null && audioSource != null && blockSoundTimer <= 0f)
        {
            audioSource.PlayOneShot(blockSound);
            blockSoundTimer = blockSoundCooldown;
        }
    }
}
