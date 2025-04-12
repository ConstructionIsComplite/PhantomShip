using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    [SerializeField] int attackDamage = 20;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float attackWindup = 0.5f;

    private float lastAttackTime;
    private bool isAttacking;

    protected override void ChaseBehavior()
    {
        base.ChaseBehavior();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(attackWindup);

        if (Vector3.Distance(transform.position, player.position) <= attackRadius)
        {
            playerHealth.TakeDamage(attackDamage);
        }

        isAttacking = false;
    }
}