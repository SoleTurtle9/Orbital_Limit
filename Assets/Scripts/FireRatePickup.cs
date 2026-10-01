using UnityEngine;

public class FireRatePickup : MonoBehaviour
{
    [SerializeField] private float boostDuration = 6f;
    [SerializeField] private float fallSpeed = 2.2f;
    [SerializeField] private float rotationSpeed = 120f;
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

        player.ActivateFireRateBoost(boostDuration);

        VfxEffects.SpawnBonusPickup(transform.position);

        if (HUDController.Instance != null)
        {
            HUDController.Instance.ShowBonusNotification("Бонус: ускоренная стрельба");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPickupSound();
        }

        Destroy(gameObject);
    }
}
