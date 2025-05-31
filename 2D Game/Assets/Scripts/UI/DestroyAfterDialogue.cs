using UnityEngine;

public class DestroyAfterDialogue : MonoBehaviour
{
    public SignInteraction dialogueScript;   // 拖拽绑定已有的 SignInteraction 脚本
    public float delayAfterTrigger = 10f;    // 延迟时间
    private bool triggered = false;

    void Update()
    {
        // 检测对话是否触发，并且只触发一次
        if (!triggered && dialogueScript != null && dialogueScriptWasTriggered())
        {
            triggered = true;
            Invoke(nameof(DestroySelf), delayAfterTrigger);
        }
    }

    void DestroySelf()
    {
        Destroy(transform.root.gameObject); // 销毁根对象
    }

    // 检测 SignInteraction 是否已显示过对话
    bool dialogueScriptWasTriggered()
    {
        // 如果对话面板和文本都激活，说明对话触发过
        return dialogueScript.dialoguePanel.activeSelf && dialogueScript.targetTextObject.activeSelf;
    }
}
