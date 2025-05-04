using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    public GameObject hintUI;   // 提示UI对象
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = hintUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = hintUI.AddComponent<CanvasGroup>();
        }

        hintUI.SetActive(false); // 默认隐藏
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintUI.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        for (float f = 0; f <= 1; f += Time.deltaTime * 5)
        {
            canvasGroup.alpha = f;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private System.Collections.IEnumerator FadeOut()
    {
        for (float f = 1; f >= 0; f -= Time.deltaTime * 5)
        {
            canvasGroup.alpha = f;
            yield return null;
        }
        canvasGroup.alpha = 0;
        hintUI.SetActive(false);
    }
}
