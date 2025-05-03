using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GemUIController : MonoBehaviour
{
    public TextMeshProUGUI gemText;
    private int gemCount = 0;

    public void AddGem(int amount)
    {
        gemCount += amount;
        UpdateGemText();
    }

    private void UpdateGemText()
    {
        gemText.text = "x " + gemCount.ToString();
    }
}
