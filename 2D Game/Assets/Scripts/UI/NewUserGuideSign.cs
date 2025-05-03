using UnityEngine;
using TMPro;

public class NewUserGuideSign : MonoBehaviour
{
    public TextMeshProUGUI hintText;         // ��ʾ�����ֿ����� HintText��
    [TextArea] public string message = "Use A and D to move. Press Space to jump!";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintText != null)
        {
            hintText.text = message;
            hintText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }
}
