using UnityEngine;

public class AmmoPickup : PickupItem
{
    [Header("Ammo Settings")]
    [SerializeField] int ammoAmount = 15;

    protected override void ApplyEffect(GameObject player)
    {
        WeaponController weapon = player.GetComponentInChildren<WeaponController>();
        if (weapon) weapon.AddAmmo(ammoAmount);
    }
}