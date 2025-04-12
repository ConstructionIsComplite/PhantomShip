using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 2f;
    [SerializeField] float projectileSpeed = 15f;
    [SerializeField] float aimingThreshold = 5f;

    private float nextFireTime;
    private Vector3 predictedPlayerPosition;

    protected override void Awake()
    {
        base.Awake();
        attackRadius = 10f; // Set appropriate ranged attack distance
    }

    protected override void ChaseBehavior()
    {
        base.ChaseBehavior();

        // Predict player position
        Vector3 playerVelocity = player.GetComponent<PlayerMovement>().CurrentVelocity;
        predictedPlayerPosition = player.position + playerVelocity * (Time.deltaTime * aimingThreshold);

        // Face predicted position
        Vector3 lookDirection = predictedPlayerPosition - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(lookDirection),
            agent.angularSpeed * Time.deltaTime
        );

        if (Time.time >= nextFireTime &&
            Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        Vector3 shootDirection = (predictedPlayerPosition - firePoint.position).normalized;
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection)
        );

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.Initialize(shootDirection, projectileSpeed);
    }
}