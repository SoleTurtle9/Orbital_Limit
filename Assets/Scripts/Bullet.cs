using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private AudioSource audioSource;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;

    private void Start()
{
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.PlayLaser();
    }

    Destroy(gameObject, lifetime);
}

    private void Update()
    {
        transform.Translate(
            Vector2.up * speed * Time.deltaTime,
            Space.Self
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Asteroid"))
            return;

        Vector3 hitPosition = transform.position;
        Asteroid asteroid = other.GetComponent<Asteroid>();

        if (hitEffectPrefab != null)
        {
            Instantiate(
                hitEffectPrefab,
                hitPosition,
                Quaternion.identity
            );
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayHit();
        }

        int scoreValue = 1;
        bool asteroidDestroyed = true;

        if (asteroid != null)
        {
            scoreValue = asteroid.ScoreValue;
            asteroidDestroyed = asteroid.TakeHit();
        }
        else
        {
            VfxEffects.SpawnAsteroidExplosion(hitPosition);
            Destroy(other.gameObject);
        }

        Destroy(gameObject);

        if (asteroidDestroyed && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
    }
}
