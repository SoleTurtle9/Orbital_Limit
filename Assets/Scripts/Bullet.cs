using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private AudioSource audioSource;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab;

    private bool hasHit;

    private void Awake()
    {
        // Projectiles detect hits without pushing the ship or other rigid bodies.
        foreach (Collider2D bulletCollider in GetComponentsInChildren<Collider2D>())
        {
            bulletCollider.isTrigger = true;
        }
    }

    public void IgnoreOwnerCollisions(GameObject owner)
    {
        Collider2D[] bulletColliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] ownerColliders = owner.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D bulletCollider in bulletColliders)
        {
            foreach (Collider2D ownerCollider in ownerColliders)
            {
                Physics2D.IgnoreCollision(bulletCollider, ownerCollider);
            }
        }
    }

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
        if (hasHit || !other.CompareTag("Asteroid"))
            return;

        hasHit = true;
        foreach (Collider2D bulletCollider in GetComponentsInChildren<Collider2D>())
        {
            bulletCollider.enabled = false;
        }

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
