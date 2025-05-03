using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        float duration = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, duration);
    }
}
