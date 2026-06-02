using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    public bool MusicEnabled => musicEnabled;
    public bool SFXEnabled => sfxEnabled;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (!musicEnabled) return;

        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled) return;

        sfxSource.PlayOneShot(clip);
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        musicSource.mute = !musicEnabled;
    }

    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        sfxSource.mute = !sfxEnabled;
    }
}