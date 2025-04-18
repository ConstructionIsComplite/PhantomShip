using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] protected float visionRadius = 15f;
    [SerializeField] protected float visionAngle = 110f;
    [SerializeField] protected float attackRadius = 3f;
    [SerializeField] protected float patrolSpeed = 2f;
    [SerializeField] protected float chaseSpeed = 5f;
    [SerializeField] protected float searchTime = 5f;
    [SerializeField] protected LayerMask targetMask;
    [SerializeField] protected LayerMask obstacleMask;

    [Header("Patrol Settings")]
    [SerializeField] protected Transform[] patrolPoints;
    [SerializeField] protected float waypointTolerance = 1f;

    [Header("Optimization")]
    [SerializeField] float visionUpdateInterval = 0.3f;

    private float visionUpdateTimer;

    protected NavMeshAgent agent;
    protected Transform player;
    protected Health playerHealth;
    protected Vector3 lastKnownPosition;
    protected int currentPatrolIndex;
    protected float searchTimer;
    protected bool isSearching;

    protected enum AIState { Patrol, Chase, Search }
    protected AIState currentState = AIState.Patrol;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent = GetComponent<NavMeshAgent>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = player.GetComponent<Health>();
        }

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    protected virtual void Update()
    {
        if (player == null || playerHealth == null || playerHealth.IsDead)
        {
            currentState = AIState.Patrol;
            return;
        }

        visionUpdateTimer -= Time.deltaTime;
        if (visionUpdateTimer <= 0)
        {
            UpdateStateTransitions();
            visionUpdateTimer = visionUpdateInterval;
        }

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Search:
                SearchBehavior();
                break;
        }

        UpdateStateTransitions();
    }

    protected virtual void UpdateStateTransitions()
    {
        bool canSeePlayer = CheckPlayerVisibility();

        if (canSeePlayer)
        {
            currentState = AIState.Chase;
            lastKnownPosition = player.position;
            searchTimer = searchTime; // ����� ������� ������
        }
        else if (currentState == AIState.Chase)
        {
            // ����������� ������� � ����� ���� ������� LOS
            currentState = AIState.Search;
            agent.SetDestination(lastKnownPosition);
        }
    }

    protected bool CheckPlayerVisibility()
    {
        if (player == null || playerHealth == null || playerHealth.IsDead) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // �������� ���������� � ����
        if (distanceToPlayer > visionRadius) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > visionAngle * 0.5f) return false;

        // ������������� Raycast
        Vector3[] checkPoints = {
        player.position, // ����������� �����
        player.position + Vector3.up * 0.5f, // ������
        player.position - Vector3.up * 0.2f  // ����
    };

        foreach (Vector3 point in checkPoints)
        {
            Vector3 dirToPoint = point - transform.position;
            if (Physics.Raycast(transform.position, dirToPoint.normalized,
                out RaycastHit hit, visionRadius, obstacleMask | targetMask))
            {
                if (hit.transform.CompareTag("Player")) return true;
            }
        }
        return false;
    }

    protected virtual void PatrolBehavior()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0) return;

        if (agent.remainingDistance <= waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    protected virtual void ChaseBehavior()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    protected virtual void SearchBehavior()
    {
        agent.speed = patrolSpeed;
        searchTimer -= Time.deltaTime;

        if (agent.remainingDistance <= waypointTolerance || searchTimer <= 0)
        {
            currentState = AIState.Patrol;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (currentState != AIState.Chase)
        {
            lastKnownPosition = player.position;
            currentState = AIState.Search;
            searchTimer = searchTime;
            agent.SetDestination(lastKnownPosition);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vision cone
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Vector3 leftDirection = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftDirection * visionRadius);
        Gizmos.DrawRay(transform.position, rightDirection * visionRadius);

        // Attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}