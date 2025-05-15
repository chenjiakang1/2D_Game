using UnityEngine;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI 元素")]
    public RectTransform fillRect; // 血条红色图层的 RectTransform
    public TextMeshProUGUI percentageText; // 显示百分比文字

    public void SetHealth(float currentHealth, float maxHealth)
    {
        float percent = Mathf.Clamp01(currentHealth / maxHealth);

        // ✅ 用 localScale 缩放，不改 sizeDelta
        if (fillRect != null)
        {
            fillRect.localScale = new Vector3(percent, 1f, 1f);
        }

        if (percentageText != null)
        {
            percentageText.text = Mathf.RoundToInt(percent * 100f) + "%";
        }
    }
}
