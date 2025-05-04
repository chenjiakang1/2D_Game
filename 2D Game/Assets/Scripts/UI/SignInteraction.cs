using UnityEngine;
using TMPro;

public class SignInteraction : MonoBehaviour
{
    public GameObject dialoguePanel;             // 对话框背景容器
    public GameObject targetTextObject;          // 你需要控制的Text对象（比如 StoryText）

    private bool isPlayerNear = false;
    private bool isDialogueVisible = false;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (targetTextObject != null)
            targetTextObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isDialogueVisible = !isDialogueVisible;

                if (dialoguePanel != null)
                    dialoguePanel.SetActive(isDialogueVisible);

                if (targetTextObject != null)
                    targetTextObject.SetActive(isDialogueVisible);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            if (targetTextObject != null)
                targetTextObject.SetActive(false);

            isDialogueVisible = false;
        }
    }
}
