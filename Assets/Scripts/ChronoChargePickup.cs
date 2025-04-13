using UnityEngine;

public class ChronoChargePickup : PickupItem
{
    [Header("Time Settings")]
    [SerializeField] float timeRestoreAmount = 0.3f;

    protected override void ApplyEffect(GameObject player)
    {
        TimeManager.Instance.SetTimeScale(
            Mathf.Clamp(
                TimeManager.Instance.CurrentTimeScale + timeRestoreAmount,
                TimeManager.Instance.MinTimeScale,
                1f
            )
        );
        
        SoundEvents.OnPlayerCharacterEffect(pickupSound);
    }
}