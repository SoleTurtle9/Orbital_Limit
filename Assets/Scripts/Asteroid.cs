using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float speedVariation = 1f;
    [SerializeField] private float speedIncreasePerScore = 0.1f;

    [Header("Durability")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int scoreValue = 1;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 60f;

    [Header("Damage")]
    [SerializeField] private int damage = 20;

    private float asteroidSpeed;
    private float rotationDirection;
    private int currentHealth;
    private bool isDestroyed;

    public int ScoreValue => scoreValue;

    private void Start()
    {
        currentHealth = maxHealth;
        asteroidSpeed = speed + Random.Range(-speedVariation, speedVariation);

        rotationDirection = Random.value < 0.5f ? -1f : 1f;

        rotationSpeed = Random.Range(30f, 90f);
    }

    private void Update()
    {
        if (isDestroyed)
            return;

        float currentSpeed = asteroidSpeed;

        if (ScoreManager.Instance != null)
        {
            currentSpeed += ScoreManager.Instance.GetScore() * speedIncreasePerScore;
        }

        // Движение вниз
        transform.Translate(
            Vector2.down * currentSpeed * Time.deltaTime,
            Space.World
        );

        // Вращение
        transform.Rotate(
            0f,
            0f,
            rotationSpeed * rotationDirection * Time.deltaTime
        );

        // Удаление за пределами экрана
        if (transform.position.y < -7f)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public bool TakeHit(int damage = 1)
    {
        if (isDestroyed)
            return false;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth > 0)
            return false;

        isDestroyed = true;
        DestroyAsteroid();

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDestroyed && other.CompareTag("Player"))
        {
            isDestroyed = true;

            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            DestroyAsteroid();
        }
    }

    private void DestroyAsteroid()
    {
        foreach (Collider2D asteroidCollider in GetComponents<Collider2D>())
        {
            asteroidCollider.enabled = false;
        }

        VfxEffects.SpawnAsteroidExplosion(transform.position);
        Destroy(gameObject);
    }
}
