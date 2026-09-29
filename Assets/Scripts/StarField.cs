using UnityEngine;

public class StarField : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private int starCount = 70;

    [SerializeField] private float minX = -9f;
    [SerializeField] private float maxX = 9f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    [SerializeField] private float minSpeed = 0.2f;
    [SerializeField] private float maxSpeed = 0.8f;

    private void Start()
    {
        for (int i = 0; i < starCount; i++)
        {
            CreateStar();
        }
    }

    private void CreateStar()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        Vector3 position = new Vector3(
            randomX,
            randomY,
            0f
        );

        GameObject star = Instantiate(
            starPrefab,
            position,
            Quaternion.identity,
            transform
        );

        float size = Random.Range(0.02f, 0.08f);
        star.transform.localScale = Vector3.one * size;

        StarMovement starMovement = star.AddComponent<StarMovement>();

        starMovement.Initialize(
            Random.Range(minSpeed, maxSpeed),
            minY
        );
    }
}