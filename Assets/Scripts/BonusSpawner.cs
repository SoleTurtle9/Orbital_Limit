using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    [Header("Bonus Prefabs")]
    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private GameObject fireRatePickupPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDelay = 10f;
    [SerializeField] private float maxSpawnDelay = 16f;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float spawnY = 6f;

    [Header("Bonus Chances")]
    [SerializeField] private float healthPickupWeight = 1f;
    [SerializeField] private float fireRatePickupWeight = 1f;

    private float spawnTimer;

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
            return;

        SpawnBonus();
        ResetSpawnTimer();
    }

    private void ResetSpawnTimer()
    {
        spawnTimer = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnBonus()
    {
        GameObject bonusPrefab = ChooseBonusPrefab();

        if (bonusPrefab == null)
            return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnY,
            0f
        );

        Instantiate(
            bonusPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private GameObject ChooseBonusPrefab()
    {
        bool hasHealthPickup = healthPickupPrefab != null && healthPickupWeight > 0f;
        bool hasFireRatePickup = fireRatePickupPrefab != null && fireRatePickupWeight > 0f;

        if (!hasHealthPickup && !hasFireRatePickup)
            return null;

        if (hasHealthPickup && !hasFireRatePickup)
            return healthPickupPrefab;

        if (!hasHealthPickup && hasFireRatePickup)
            return fireRatePickupPrefab;

        float totalWeight = healthPickupWeight + fireRatePickupWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue < healthPickupWeight)
            return healthPickupPrefab;

        return fireRatePickupPrefab;
    }
}
