using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int restoreAmount = 25;
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float destroyY = -7f;

    private void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        if (player.RestoreHealth(restoreAmount))
        {
            VfxEffects.SpawnBonusPickup(transform.position);

            if (HUDController.Instance != null)
            {
                HUDController.Instance.ShowBonusNotification("Бонус: восстановление здоровья +" + restoreAmount + " HP");
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPickupSound();
            }

            Destroy(gameObject);
        }
    }
}
