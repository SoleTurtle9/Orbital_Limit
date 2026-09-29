using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.15f;
    [SerializeField] private float maxScale = 0.5f;

    private float timer;
    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        float progress = timer / lifetime;

        transform.localScale = Vector3.Lerp(
            startScale,
            Vector3.one * maxScale,
            progress
        );

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}