using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public WoodBoxCollector woodBoxCollector;
    public int requiredWood = 6;

    public GameObject hiddenObject;                   // 要显示或隐藏的物体（如桥或障碍）
    public bool revealInsteadOfHide = true;           // ✅ true: 显示物体，false: 隐藏物体

    public TextMeshProUGUI defaultText;               // 木头足够时的提示文字
    public TextMeshProUGUI insufficientText;          // 木头不足时的提示文字
    public GameObject extraImageObject;               // 要隐藏/恢复的图片

    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CheckWoodCount();
            }

            // 按 F 隐藏“木头不足”提示并恢复图片
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (insufficientText != null)
                    insufficientText.gameObject.SetActive(false);

                if (extraImageObject != null)
                    extraImageObject.SetActive(true);
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

            if (woodBoxCollector.woodCount >= requiredWood)
            {
                if (defaultText != null)
                    defaultText.gameObject.SetActive(true);

                if (insufficientText != null)
                    insufficientText.gameObject.SetActive(false);
            }
            else
            {
                if (defaultText != null)
                    defaultText.gameObject.SetActive(false);

                if (insufficientText != null)
                    insufficientText.gameObject.SetActive(false);

                if (extraImageObject != null)
                    extraImageObject.SetActive(true); // 恢复图片
            }
        }
    }

    void CheckWoodCount()
    {
        if (woodBoxCollector.woodCount >= requiredWood)
        {
            // ✅ 根据开关设置物体是显示还是隐藏
            if (hiddenObject != null)
                hiddenObject.SetActive(revealInsteadOfHide);

            if (defaultText != null)
                defaultText.gameObject.SetActive(true);

            if (insufficientText != null)
                insufficientText.gameObject.SetActive(false);

            if (extraImageObject != null)
                extraImageObject.SetActive(true);

            // 销毁 NPC 的父对象或自身
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }
        else
        {
            if (defaultText != null)
                defaultText.gameObject.SetActive(false);

            if (insufficientText != null)
                insufficientText.gameObject.SetActive(true);

            if (extraImageObject != null)
                extraImageObject.SetActive(false);
        }
    }
}
