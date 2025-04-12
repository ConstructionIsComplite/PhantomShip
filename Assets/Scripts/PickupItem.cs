using UnityEngine;

public abstract class PickupItem : MonoBehaviour
{
    [Header("Base Pickup Settings")]
    [SerializeField] protected float rotationSpeed = 50f;
    [SerializeField] protected GameObject pickupEffect;
    [SerializeField] protected AudioClip pickupSound;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
            PlayEffects();
            Destroy(gameObject);
        }
    }

    protected virtual void PlayEffects()
    {
        if (pickupEffect) Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (pickupSound) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }

    protected abstract void ApplyEffect(GameObject player);
}