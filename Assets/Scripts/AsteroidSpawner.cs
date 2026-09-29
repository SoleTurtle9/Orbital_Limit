using UnityEngine;
public class AsteroidSpawner : MonoBehaviour { [SerializeField] private GameObject asteroidPrefab; [SerializeField] private float spawnInterval = 2f; [SerializeField] private float minSpawnInterval = 0.9f; [SerializeField] private float intervalDecreasePerScore = 0.02f;
private float spawnTimer;

private void Start()
{
    spawnTimer = 1f;
}

private void Update()
{
    spawnTimer -= Time.deltaTime;

    if (spawnTimer <= 0f)
    {
        SpawnAsteroid();
        spawnTimer = GetCurrentSpawnInterval();
    }
}

private float GetCurrentSpawnInterval()
{
    float currentInterval = spawnInterval;

    if (ScoreManager.Instance != null)
    {
        currentInterval -= ScoreManager.Instance.GetScore() * intervalDecreasePerScore;
    }

    return Mathf.Max(currentInterval, minSpawnInterval);
}

private void SpawnAsteroid()
{
    float randomX = Random.Range(-8f, 8f);

    Vector3 spawnPosition = new Vector3(
        randomX,
        6f,
        0f
    );

    Instantiate(
        asteroidPrefab,
        spawnPosition,
        Quaternion.identity
    );
}
}