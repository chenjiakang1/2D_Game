using UnityEngine;
using TMPro;

public class SignInteraction : MonoBehaviour
{
    public GameObject PressFText;                // 提示文字对象（TextMeshProUGUI）
    public GameObject dialoguePanel;             // 新增：对话框背景容器
    public TextMeshProUGUI dialogueText;         // 对话框文字
    [TextArea]
    public string dialogue =
     "Does the world have an end?\n\n" +
     "We keep running—forward, upward, into the unknown—\n\n" +
     "but we rarely stop to ask:\n\n" +
     "Why did I begin this journey in the first place?";

    public Vector3 offset = new Vector3(0, 1.5f, 0); // 提示文字位置偏移

    private RectTransform canvasRect;
    private RectTransform textRect;
    private bool isPlayerNear = false;
    private bool isDialogueVisible = false;

    void Start()
    {
        if (PressFText != null)
        {
            PressFText.SetActive(false);
            textRect = PressFText.GetComponent<RectTransform>();
            canvasRect = PressFText.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear)
        {
            UpdateTextPosition();

            if (Input.GetKeyDown(KeyCode.F))
            {
                isDialogueVisible = !isDialogueVisible;

                if (dialoguePanel != null)
                    dialoguePanel.SetActive(isDialogueVisible);

                if (dialogueText != null)
                    dialogueText.text = dialogue;

                Debug.Log("F pressed: toggle dialogue");
            }
        }
    }

    private void UpdateTextPosition()
    {
        if (textRect == null || canvasRect == null)
            return;

        Vector3 worldPos = transform.position + offset;
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);

        Vector2 anchoredPos = new Vector2(
            (viewportPos.x - 0.5f) * canvasRect.sizeDelta.x,
            (viewportPos.y - 0.5f) * canvasRect.sizeDelta.y
        );

        textRect.anchoredPosition = anchoredPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;

            if (PressFText != null)
            {
                PressFText.SetActive(true);
                UpdateTextPosition(); // 立刻更新一次位置
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            if (PressFText != null)
                PressFText.SetActive(false);

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            isDialogueVisible = false;
        }
    }
}
