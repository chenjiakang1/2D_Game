using UnityEngine;
using TMPro;

public class NPCInteraction : MonoBehaviour
{
    public WoodBoxCollector woodBoxCollector;
    public int requiredWood = 6;

    public GameObject hiddenObject;                   // 要显示的隐藏物体（如桥）
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

            // 按 F 时隐藏“木头不足”提示并恢复图片
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
                defaultText.gameObject.SetActive(true);
                insufficientText.gameObject.SetActive(false);
            }
            else
            {
                defaultText.gameObject.SetActive(false);
                insufficientText.gameObject.SetActive(false);
                extraImageObject.SetActive(true);  // 离开时恢复图片
            }
        }
    }

    void CheckWoodCount()
    {
        if (woodBoxCollector.woodCount >= requiredWood)
        {
            if (hiddenObject != null)
                hiddenObject.SetActive(true);

            if (defaultText != null)
                defaultText.gameObject.SetActive(true);

            if (insufficientText != null)
                insufficientText.gameObject.SetActive(false);

            if (extraImageObject != null)
                extraImageObject.SetActive(true);

            // ✅ 销毁 NPC 的父对象（Rabbit_Npc 的 parent）
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);  // 如果无父对象则销毁自身
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
