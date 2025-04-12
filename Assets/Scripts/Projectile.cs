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
        this.speed = projectileSpeed;

        // Для kinematic rigidbody используем MovePosition
        if (rb.isKinematic)
        {
            StartCoroutine(KinematicMovement());
        }
        else
        {
            rb.velocity = direction * speed;
        }
    }

    private IEnumerator KinematicMovement()
    {
        while (true)
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
    }

    void FixedUpdate()
    {
        MaintainHorizontalTrajectory();
    }

    void MaintainHorizontalTrajectory()
    {
        if (rb.isKinematic)
        {
            // Для kinematic движения обновляем позицию
            Vector3 newPos = transform.position + direction * speed * Time.fixedDeltaTime;
            newPos.y = initialY;
            rb.MovePosition(newPos);
        }
        else
        {
            // Оригинальная физическая логика
            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = 0;
            rb.velocity = currentVelocity.normalized * speed;

            Vector3 newPosition = rb.position;
            newPosition.y = initialY;
            rb.position = newPosition;
        }
    }
    /*

    void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision);
        Destroy(gameObject);
    }
    */

    void OnTriggerEnter(Collider other)
    {
        // Нанесение урона
        Health health = other.GetComponent<Health>();
        if (health) health.TakeDamage(damage);

        // Эффекты и уничтожение снаряда
        ProcessCollision(other.gameObject);
        Destroy(gameObject);
    }

    void ProcessCollision(GameObject target)
    {
        SpawnImpactEffect(target.transform.position); // Передаём позицию цели
        TryDamageTarget(target);
    }

    void SpawnImpactEffect(Vector3 position) 
    {
        if (!impactEffect) return;

        Instantiate(
            impactEffect,
            position, // Используем позицию столкновения
            Quaternion.identity
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
