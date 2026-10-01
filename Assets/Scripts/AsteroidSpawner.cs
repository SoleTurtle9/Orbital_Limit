using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Spawning")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.9f;

    [Header("Waves")]
    [SerializeField] private float waveDuration = 20f;
    [SerializeField] private float intervalDecreasePerWave = 0.1f;

    private float spawnTimer;
    private float waveTimer;
    private int currentWave = 1;

    public int CurrentWave => currentWave;

    private void Start()
    {
        spawnTimer = 1f;
        waveTimer = waveDuration;

        Debug.Log("Wave " + currentWave + " started");
    }

    private void Update()
    {
        UpdateWave();

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnAsteroid();
            spawnTimer = GetCurrentSpawnInterval();
        }
    }

    private void UpdateWave()
    {
        waveTimer -= Time.deltaTime;

        if (waveTimer > 0f)
            return;

        currentWave++;
        waveTimer += waveDuration;

        Debug.Log(
            "Wave " + currentWave + " started. Spawn interval: " +
            GetCurrentSpawnInterval().ToString("0.00") + " seconds"
        );
    }

    private float GetCurrentSpawnInterval()
    {
        float currentInterval = spawnInterval -
            (currentWave - 1) * intervalDecreasePerWave;

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
