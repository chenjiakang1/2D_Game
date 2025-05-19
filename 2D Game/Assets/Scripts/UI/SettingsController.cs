using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;

    private bool isPaused = false;

    private void Start()
    {
        settingsPanel.SetActive(false);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        volumeSlider.value = AudioListener.volume;
    }

    public void ToggleSettings()
    {
        isPaused = !settingsPanel.activeSelf;
        settingsPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;  // 暂停 or 继续游戏
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;  // 恢复时间
    }

    private void OnDisable()
    {
        // 防止意外情况下面板未手动关闭导致游戏卡死
        Time.timeScale = 1f;
    }
}
