using UnityEngine;
using TMPro;  // 如果你用的是 TextMeshPro

public class WoodBoxCollector : MonoBehaviour
{
    public int woodCount = 0;  // 当前木头数量
    public TextMeshProUGUI woodCountText;  // UI文本组件，显示数量

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Triggered: " + other.name);
        if (other.CompareTag("WoodBox"))
        {
            woodCount++;
            UpdateWoodUI();
            Destroy(other.gameObject);  // 移除木箱子
        }
    }

    void UpdateWoodUI()
    {
        woodCountText.text = ": " + woodCount.ToString();
    }
}
