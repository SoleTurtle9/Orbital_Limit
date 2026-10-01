using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const string SfxVolumeKey = "SfxVolume";

    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

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
