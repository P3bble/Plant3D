using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] GameObject settingsPanel;

    public static float MouseSensitivity { get; private set; } = 1f;
    public static bool IsOpen { get; private set; } = false;

    const string VOL_KEY = "volume";
    const string SENS_KEY = "sensitivity";
    const string FULL_KEY = "fullscreen";

    void Awake()
    {
        // Ensure EventSystem exists
        if (!FindObjectOfType<EventSystem>())
        {
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(go);
        }

        // Hard-wire listeners (works even if you forget Inspector wiring)
        if (volumeSlider) volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        if (sensitivitySlider) sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        if (fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

        // Sensible slider ranges so you FEEL a difference
        if (sensitivitySlider)
        {
            if (sensitivitySlider.minValue == 0f) sensitivitySlider.minValue = 0.2f;
            if (sensitivitySlider.maxValue == 1f) sensitivitySlider.maxValue = 2.0f;
        }
    }

    void Start()
    {
        // Load saved
        float vol = PlayerPrefs.GetFloat(VOL_KEY, 1f);
        float sens = PlayerPrefs.GetFloat(SENS_KEY, 1f);
        bool full = PlayerPrefs.GetInt(FULL_KEY, Screen.fullScreen ? 1 : 0) == 1;

        // Apply
        AudioListener.volume = vol;
        MouseSensitivity = sens;
        ApplyFullscreen(full);

        // Push to UI
        if (volumeSlider) volumeSlider.value = vol;
        if (sensitivitySlider) sensitivitySlider.value = sens;
        if (fullscreenToggle) fullscreenToggle.isOn = full;

        // Ensure panel starts closed
        if (settingsPanel) { settingsPanel.SetActive(false); IsOpen = false; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePanel();
    }

    void TogglePanel()
    {
        if (!settingsPanel) return;

        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
        IsOpen = !isActive;

        // Pause world + show cursor
        Time.timeScale = IsOpen ? 0f : 1f;
        Cursor.lockState = IsOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsOpen;

        Debug.Log($"[Settings] Panel {(IsOpen ? "OPEN" : "CLOSED")}");
    }

    public void OnVolumeChanged(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat(VOL_KEY, v);
        Debug.Log($"[Settings] Volume -> {v:0.00}");
    }

    public void OnSensitivityChanged(float s)
    {
        MouseSensitivity = s; // 0.2–2.0 range recommended
        PlayerPrefs.SetFloat(SENS_KEY, s);
        Debug.Log($"[Settings] Sensitivity -> {s:0.00}");
    }

    public void OnFullscreenToggled(bool on)
    {
        ApplyFullscreen(on);
        PlayerPrefs.SetInt(FULL_KEY, on ? 1 : 0);
        Debug.Log($"[Settings] Fullscreen -> {on}");
    }

    void ApplyFullscreen(bool on)
    {
        // In Editor, this won’t truly fullscreen the Game view. Works in a build.
        Screen.fullScreenMode = on ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = on;
    }

    // If you use buttons:
    public void OpenSettings() { if (!settingsPanel) return; if (!settingsPanel.activeSelf) TogglePanel(); }
    public void CloseSettings() { if (!settingsPanel) return; if (settingsPanel.activeSelf) TogglePanel(); PlayerPrefs.Save(); }
}
