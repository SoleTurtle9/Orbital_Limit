using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float drag = 1.5f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Boundaries")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4.5f;
    [SerializeField] private float maxY = 4.5f;

    [Header("Shooting")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    private Rigidbody2D rb;
    private float rotationInput;
    private float thrustInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        thrustInput = Input.GetAxisRaw("Vertical");
        rotationInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
        KeepInsideBounds();
    }

    private void HandleRotation()
    {
        float rotation = -rotationInput * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotation);
    }

    private void HandleMovement()
    {
        Vector2 forward = transform.up;

        if (thrustInput > 0)
        {
            rb.AddForce(forward * thrustInput * acceleration);
        }
        else if (thrustInput < 0)
        {
            rb.AddForce(forward * thrustInput * acceleration);
        }

        rb.linearDamping = drag;

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void KeepInsideBounds()
    {
        Vector2 position = rb.position;
        Vector2 velocity = rb.linearVelocity;

        if (position.x < minX)
        {
            position.x = minX;
            velocity.x = 0;
        }
        else if (position.x > maxX)
        {
            position.x = maxX;
            velocity.x = 0;
        }

        if (position.y < minY)
        {
            position.y = minY;
            velocity.y = 0;
        }
        else if (position.y > maxY)
        {
            position.y = maxY;
            velocity.y = 0;
        }

        rb.position = position;
        rb.linearVelocity = velocity;
    }

    private void Shoot()
    {
        Instantiate(
            laserPrefab,
            firePoint.position,
            firePoint.rotation
        );
    }
}