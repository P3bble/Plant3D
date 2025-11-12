using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject playerUI;

    public static float MouseSensitivity { get; private set; } = 1f;
    public static bool IsOpen { get; private set; } = false;

    const string VOL_KEY = "volume";
    const string SENS_KEY = "sensitivity";
    const string FULL_KEY = "fullscreen";

    void Start()
    {
 
        float vol = PlayerPrefs.GetFloat(VOL_KEY, 1f);
        float sens = PlayerPrefs.GetFloat(SENS_KEY, 1f);
        bool full = PlayerPrefs.GetInt(FULL_KEY, Screen.fullScreen ? 1 : 0) == 1;
        AudioListener.volume = vol;
        MouseSensitivity = sens;
        Screen.fullScreen = full;

        // UI
        volumeSlider.value = vol;
        sensitivitySlider.value = sens;
        fullscreenToggle.isOn = full;

        // listeners
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

        // Start hidden
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePanel();
    }

    void TogglePanel()
    {
        bool showing = !settingsPanel.activeSelf;
        settingsPanel.SetActive(showing);
        IsOpen = showing;

        if (playerUI) playerUI.SetActive(!showing);

        Time.timeScale = showing ? 0f : 1f;
        Cursor.lockState = showing ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showing;
    }

    void OnVolumeChanged(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat(VOL_KEY, v);
    }

    void OnSensitivityChanged(float s)
    {
        MouseSensitivity = s;
        PlayerPrefs.SetFloat(SENS_KEY, s);
    }

    void OnFullscreenToggled(bool on)
    {
        Screen.fullScreen = on;
        PlayerPrefs.SetInt(FULL_KEY, on ? 1 : 0);
    }
}
