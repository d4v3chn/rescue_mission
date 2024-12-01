using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public Transform[] patrolPoints; 
    public Transform gawe;
    public float chaseSpeed = 5f; 
    public float patrolSpeed = 2f;
    public float originalRadius = 5f;
    public float increasedRadius = 8f;

    public GameManager GM;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private CircleCollider2D detectionCollider;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the Dragon GameObject.");
            return;
        }

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        detectionCollider = GetComponent<CircleCollider2D>();
        if (detectionCollider == null)
        {
            Debug.LogError("CircleCollider2D is missing on the Dragon GameObject.");
            return;
        }

        detectionCollider.radius = originalRadius;
    }

    void Start()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh or is missing.");
            return;
        }

        agent.speed = patrolSpeed;
        GotoNextPatrolPoint();
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (isChasing)
        {
            ChaseGawe();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (agent.pathPending || agent.remainingDistance > 0.5f)
            return;

        GotoNextPatrolPoint();
    }

    private void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning($"Patrol point {currentPatrolIndex} is not on the NavMesh.");
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void ChaseGawe()
    {
        if (gawe == null) return;

        agent.SetDestination(gawe.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == gawe)
        {
            isChasing = true;
            agent.speed = chaseSpeed;

            detectionCollider.radius = increasedRadius;

            Debug.Log("Gawe detected! Dragon is now chasing.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == gawe)
        {
            isChasing = false;
            agent.speed = patrolSpeed;

            detectionCollider.radius = originalRadius;

            ResumePatrol();

            Debug.Log("Gawe escaped! Dragon is returning to patrol.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == gawe)
        {
            Debug.Log("Gawe caught by the dragon!");

            if (GM != null)
            {
                GM.Death();
            }
            else
            {
                Debug.LogError("GameManager reference is missing in Dragon script.");
            }
        }
    }

    private void ResumePatrol()
    {
        GotoNextPatrolPoint();
    }
}
