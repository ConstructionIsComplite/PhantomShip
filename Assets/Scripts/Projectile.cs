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

        // ��� kinematic rigidbody ���������� MovePosition
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
            // ��� kinematic �������� ��������� �������
            Vector3 newPos = transform.position + direction * speed * Time.fixedDeltaTime;
            newPos.y = initialY;
            rb.MovePosition(newPos);
        }
        else
        {
            // ������������ ���������� ������
            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = 0;
            rb.velocity = currentVelocity.normalized * speed;

            Vector3 newPosition = rb.position;
            newPosition.y = initialY;
            rb.position = newPosition;
        }
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        // ��������� �����
        Health health = other.GetComponent<Health>();
        if (health) health.TakeDamage(damage);

        // ������� � ����������� �������
        ProcessCollision(other.gameObject);
        Destroy(gameObject);
    }
    */

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        SpawnImpactEffect(contact.point, contact.normal);

        // ��������� ��������� ������ �������
        rb.isKinematic = true;
        rb.detectCollisions = false;
        
       // collision.gameObject.GetComponent<Health>()?.TakeDamage(damage);

        TryDamageTarget(collision.gameObject);
        Destroy(gameObject);
    }

    /*

    void ProcessCollision(GameObject target)
    {
        SpawnImpactEffect(target.transform.position); // ������� ������� ����
        TryDamageTarget(target);
    }
    */
    /*
    void SpawnImpactEffect(Vector3 position) 
    {
        if (!impactEffect) return;

        Instantiate(
            impactEffect,
            position, // ���������� ������� ������������
            Quaternion.identity
        );
    }
    */

    void SpawnImpactEffect(Vector3 position, Vector3 normal)
    {
        if (!impactEffect) return;

        Instantiate(
            impactEffect,
            position,
            Quaternion.LookRotation(normal) // ���������� ���������� �������
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
