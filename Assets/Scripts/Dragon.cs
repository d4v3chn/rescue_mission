using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{

    // Public variables - mostly to be modified directly from Unity
    public Transform[] patrolPoints; 
    public Transform gawe;
    public float chaseSpeed = 5f; 
    public float patrolSpeed = 2f;
    public float originalRadius = 3.5f;
    public float increasedRadius = 5f;

    // Private variables for agent, patrol points and chase methods
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

        // Make the agent appear
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

    private void Start()
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
        {
            return;
        }
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
        {
            return;
        }

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

    private void ChaseGawe()
    {
        if (gawe == null) 
        { 
            return; 
        }

        agent.SetDestination(gawe.position);
    }


    private void ResumePatrol()
    {
        GotoNextPatrolPoint();
    }
}
