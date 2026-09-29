using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private void Update()
    {
        transform.Translate(
            Vector2.down * speed * Time.deltaTime,
            Space.World
        );

        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Корабль столкнулся с астероидом!");
    }
}
}