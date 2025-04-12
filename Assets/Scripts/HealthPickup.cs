using UnityEngine;

public class HealthPickup : PickupItem
{
    [Header("Health Settings")]
    [SerializeField] int healAmount = 30;

    protected override void ApplyEffect(GameObject player)
    {
        Health health = player.GetComponent<Health>();
        if (health) health.Heal(healAmount);
    }
}