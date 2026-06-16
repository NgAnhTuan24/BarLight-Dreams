using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettingsUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text screenModeText;
    [SerializeField] private TMP_Text resolutionText;
    [SerializeField] private TMP_Text vSyncText;

    [Header("Interact")]
    [SerializeField] private Button previousResolutionButton;
    [SerializeField] private Button nextResolutionButton;

    private readonly FullScreenMode[] screenModes =
    {
        FullScreenMode.Windowed,
        FullScreenMode.FullScreenWindow
    };

    private readonly string[] screenModeNames =
    {
        "Windowed",
        "Fullscreen"
    };

    private readonly string[] vSyncOptions =
    {
        "Off",
        "On"
    };

    private readonly Vector2Int[] resolutions =
    {
        new(1920, 1080),
        new(1600, 900),
        new(1366, 768),
        new(1280, 720),
    };

    private int currentScreenModeIndex;
    private int currentResolutionIndex;
    private int currentVSyncIndex;

    private void Start()
    {
        currentScreenModeIndex = Screen.fullScreenMode == FullScreenMode.Windowed ? 0 : 1;
        
        currentResolutionIndex = GetCurrentResolutionIndex();

        currentVSyncIndex = QualitySettings.vSyncCount > 0 ? 1 : 0;

        RefreshUI();
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].x == Screen.width &&
                resolutions[i].y == Screen.height)
            {
                return i;
            }
        }

        return resolutions.Length - 1;
    }

    private void RefreshUI()
    {
        screenModeText.text = screenModeNames[currentScreenModeIndex];

        bool isWindowed = screenModes[currentScreenModeIndex] == FullScreenMode.Windowed;

        if (isWindowed)
        {
            Vector2Int resolution = resolutions[currentResolutionIndex];
            resolutionText.text = $"{resolution.x}x{resolution.y}";
        }
        else
        {
            resolutionText.text = "Native";
        }

        vSyncText.text = vSyncOptions[currentVSyncIndex];

        previousResolutionButton.interactable = isWindowed;
        nextResolutionButton.interactable = isWindowed;
    }

    #region Screen Mode

    public void PreviousScreenMode()
    {
        currentScreenModeIndex--;

        if (currentScreenModeIndex < 0)
        {
            currentScreenModeIndex = screenModes.Length - 1;
        }

        ApplyScreenMode();
    }

    public void NextScreenMode()
    {
        currentScreenModeIndex++;

        if (currentScreenModeIndex >= screenModes.Length)
        {
            currentScreenModeIndex = 0;
        }

        ApplyScreenMode();
    }

    private void ApplyScreenMode()
    {
        bool isWindowed = screenModes[currentScreenModeIndex] == FullScreenMode.Windowed;

        if (isWindowed)
        {
            Vector2Int resolution = resolutions[currentResolutionIndex];

            Screen.SetResolution(resolution.x, resolution.y, FullScreenMode.Windowed);
        }
        else
        {
            Resolution desktop = Screen.currentResolution;

            Screen.SetResolution(desktop.width, desktop.height, FullScreenMode.FullScreenWindow);
        }

        RefreshUI();
    }

    #endregion

    #region Resolution

    public void PreviousResolution()
    {
        currentResolutionIndex--;

        if (currentResolutionIndex < 0)
        {
            currentResolutionIndex = resolutions.Length - 1;
        }

        ApplyResolution();
    }

    public void NextResolution()
    {
        currentResolutionIndex++;

        if (currentResolutionIndex >= resolutions.Length)
        {
            currentResolutionIndex = 0;
        }

        ApplyResolution();
    }

    private void ApplyResolution()
    {
        if (screenModes[currentScreenModeIndex] != FullScreenMode.Windowed)
        {
            return;
        }

        Vector2Int resolution = resolutions[currentResolutionIndex];

        Screen.SetResolution(resolution.x, resolution.y, FullScreenMode.Windowed);

        RefreshUI();
    }

    #endregion

    #region VSync

    public void PreviousVSync()
    {
        ToggleVSync();
    }

    public void NextVSync()
    {
        ToggleVSync();
    }

    private void ToggleVSync()
    {
        currentVSyncIndex = currentVSyncIndex == 0 ? 1 : 0;

        QualitySettings.vSyncCount = currentVSyncIndex;

        RefreshUI();
    }

    #endregion
}