using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float speedVariation = 1f;
    [SerializeField] private float speedIncreasePerScore = 0.1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 60f;

    private float asteroidSpeed;
    private float rotationDirection;

    private void Start()
    {
        asteroidSpeed = speed + Random.Range(-speedVariation, speedVariation);

        rotationDirection = Random.value < 0.5f ? -1f : 1f;

        rotationSpeed = Random.Range(30f, 90f);
    }

    private void Update()
    {
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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
}