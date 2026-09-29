using UnityEngine;

public class StarMovement : MonoBehaviour
{
    private float speed;
    private float minY;

    public void Initialize(float starSpeed, float minimumY)
    {
        speed = starSpeed;
        minY = minimumY;
    }

    private void Update()
    {
        transform.Translate(
            Vector2.down * speed * Time.deltaTime,
            Space.World
        );

        if (transform.position.y < minY)
        {
            transform.position = new Vector3(
                transform.position.x,
                5f,
                transform.position.z
            );
        }
    }
}