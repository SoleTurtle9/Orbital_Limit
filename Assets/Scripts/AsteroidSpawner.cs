using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float spawnInterval = 2f;

    private void Start()
    {
        InvokeRepeating(
            nameof(SpawnAsteroid),
            1f,
            spawnInterval
        );
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