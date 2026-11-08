using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string SfxVolumeKey = "SfxVolume";
    private const string MusicVolumeKey = "MusicVolume";

    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip laserSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip pickupSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    public void PlayLaser()
    {
        PlaySound(laserSound);
    }

    public void PlayPickupSound()
    {
        PlaySound(pickupSound);
    }

    public void PlayHit()
    {
        PlaySound(hitSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip, PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
    }
}
