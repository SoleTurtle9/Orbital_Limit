using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Asteroid Spawning")]
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject heavyAsteroidPrefab;
    [SerializeField] private GameObject fastAsteroidPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.9f;

    [Header("Heavy Asteroid")]
    [SerializeField] private int heavyAsteroidStartWave = 5;
    [SerializeField] private float heavyAsteroidChance = 0.12f;
    [SerializeField] private float heavyAsteroidChanceIncreasePerWave = 0.025f;
    [SerializeField] private float maxHeavyAsteroidChance = 0.28f;

    [Header("Fast Asteroid")]
    [SerializeField] private int fastAsteroidStartWave = 3;
    [SerializeField] private float fastAsteroidChance = 0.14f;
    [SerializeField] private float fastAsteroidChanceIncreasePerWave = 0.03f;
    [SerializeField] private float maxFastAsteroidChance = 0.35f;

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

        Instantiate(ChooseAsteroidPrefab(), spawnPosition, Quaternion.identity);
    }

    private GameObject ChooseAsteroidPrefab()
    {
        float heavyChance = GetAsteroidChance(
            heavyAsteroidPrefab,
            heavyAsteroidStartWave,
            heavyAsteroidChance,
            heavyAsteroidChanceIncreasePerWave,
            maxHeavyAsteroidChance
        );

        float fastChance = GetAsteroidChance(
            fastAsteroidPrefab,
            fastAsteroidStartWave,
            fastAsteroidChance,
            fastAsteroidChanceIncreasePerWave,
            maxFastAsteroidChance
        );

        float randomValue = Random.value;

        if (randomValue < heavyChance)
            return heavyAsteroidPrefab;

        if (randomValue < heavyChance + fastChance)
            return fastAsteroidPrefab;

        return asteroidPrefab;
    }

    private float GetAsteroidChance(
        GameObject prefab,
        int startWave,
        float baseChance,
        float chanceIncreasePerWave,
        float maxChance
    )
    {
        if (prefab == null || currentWave < startWave)
            return 0f;

        float waveBonus = (currentWave - startWave) * chanceIncreasePerWave;

        return Mathf.Min(baseChance + waveBonus, maxChance);
    }
}
