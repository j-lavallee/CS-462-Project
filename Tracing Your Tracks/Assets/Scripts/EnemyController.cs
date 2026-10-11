using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrolling,
    Following,
    Attacking
}

public class EnemyController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Renderer[] enemyRenderers;

    [Header("Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float viewAngle = 180f;
    [SerializeField] private float losePlayerTime = 3f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyState state = EnemyState.Patrolling;
    private int currentPatrolIndex;
    private bool isWaiting;
    private float timeSinceLostPlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (enemyRenderers == null || enemyRenderers.Length == 0)
        {
            enemyRenderers = GetComponentsInChildren<Renderer>();
        }
    }

    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case EnemyState.Patrolling:
                Patrol();

                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    state = EnemyState.Following;
                }

                break;

            case EnemyState.Following:
                FollowPlayer();

                if (!CanSeePlayer())
                {
                    timeSinceLostPlayer += Time.deltaTime;

                    if (timeSinceLostPlayer >= losePlayerTime)
                    {
                        state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    timeSinceLostPlayer = 0f;
                }

                break;

        }

        UpdateAnimations();
    }

    private void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }

    private void Patrol()
    {
        if (isWaiting) return;
        if (!agent.pathPending && agent.remainingDistance <= stopAtDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        GoToNextPatrolPoint();
        isWaiting = false;
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        currentPatrolIndex = closestIndex;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

    }

    private void UpdateAnimations()
    {
        var isWalking = agent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool(IsWalking, isWalking);
    }

    private bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    private bool IsFacingPlayer()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward, dirToPlayer);

        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }

    public void setMaterial(Material newMaterial)
    {
        if (enemyRenderers == null) return;

        foreach (Renderer renderer in enemyRenderers)
        {
            if (renderer == null) continue;

            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = newMaterial;
            }

            renderer.materials = materials;
        }
    }
}