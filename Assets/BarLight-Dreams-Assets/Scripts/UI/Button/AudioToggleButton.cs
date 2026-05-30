using UnityEngine;
using UnityEngine.UI;

public enum AudioType
{
    Music,
    SFX
}

public class AudioToggleButton : MonoBehaviour
{
    [SerializeField] private AudioType audioType;

    [Header("UI")]
    [SerializeField] private Image icon;

    [Header("Sprites")]
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;

    private void Start()
    {
        UpdateIcon();
    }

    public void ToggleAudio()
    {
        if (audioType == AudioType.Music)
        {
            AudioManager.instance.ToggleMusic();
        }
        else
        {
            AudioManager.instance.ToggleSFX();
        }

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        bool enabled = audioType == AudioType.Music ? AudioManager.instance.MusicEnabled : AudioManager.instance.SFXEnabled;

        icon.sprite = enabled ? on : off;
    }
}
