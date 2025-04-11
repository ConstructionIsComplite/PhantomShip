using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float speed = 20f;
    [SerializeField] int damage = 10;
    [SerializeField] LayerMask damageLayers;

    [Header("Effects")]
    [SerializeField] GameObject impactEffect;

    private Rigidbody rb;
    private Vector3 direction;
    private float initialY;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialY = transform.position.y;
        ConfigureRigidbody();
        Destroy(gameObject, lifeTime);
    }

    void ConfigureRigidbody()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.useGravity = false;
    }

    public void Initialize(Vector3 shootDirection, float projectileSpeed)
    {
        direction = new Vector3(shootDirection.x, 0, shootDirection.z).normalized;
        speed = projectileSpeed;
        rb.velocity = direction * speed;
    }

    void FixedUpdate()
    {
        MaintainHorizontalTrajectory();
    }

    void MaintainHorizontalTrajectory()
    {
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.y = 0;
        rb.velocity = currentVelocity.normalized * speed;

        Vector3 newPosition = rb.position;
        newPosition.y = initialY;
        rb.position = newPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision);
        Destroy(gameObject);
    }

    void ProcessCollision(Collision collision)
    {
        SpawnImpactEffect(collision.contacts[0]);
        TryDamageTarget(collision.gameObject);
    }

    void SpawnImpactEffect(ContactPoint contact)
    {
        if (!impactEffect) return;

        Instantiate(
            impactEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
    }

    void TryDamageTarget(GameObject target)
    {
        if (!IsValidDamageTarget(target)) return;

        Health health = target.GetComponent<Health>();
        if (health) health.TakeDamage(damage);
    }

    bool IsValidDamageTarget(GameObject target)
    {
        return (damageLayers.value & (1 << target.layer)) != 0;
    }
}
