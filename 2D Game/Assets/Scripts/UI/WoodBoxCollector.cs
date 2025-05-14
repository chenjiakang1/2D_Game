using UnityEngine;
using TMPro;

public class WoodBoxCollector : MonoBehaviour
{
    public int woodCount = 0;
    public TextMeshProUGUI woodCountText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WoodBox"))
        {
            WoodBox box = other.GetComponent<WoodBox>();
            if (box == null || box.isCollected)
                return;

            box.isCollected = true;  // ✅ 标记为已拾取，防止重复

            woodCount++;
            UpdateWoodUI();
            Destroy(other.gameObject);
        }
    }

    void UpdateWoodUI()
    {
        woodCountText.text = ": " + woodCount.ToString();
    }
}
