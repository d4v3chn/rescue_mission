using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points for patrolling
    public float chaseSpeed = 4f;    // Speed while chasing
    public float patrolSpeed = 2f;  // Speed while patrolling
    public Transform target;        // Drag-and-drop GameObject reference for the target (e.g., Gawe)

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh. Ensure the dragon starts on a baked NavMesh.");
            return;
        }

        agent.speed = patrolSpeed;
        GotoNextPatrolPoint();
    }

    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        if (isChasing)
        {
            ChaseTarget();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (!agent.isOnNavMesh || agent.pathPending)
            return;

        if (agent.remainingDistance < 0.5f)
        {
            GotoNextPatrolPoint();
        }
    }

    private void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        if (NavMesh.SamplePosition(patrolTarget.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Patrol point is not on the NavMesh.");
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void ChaseTarget()
    {
        if (target != null && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position); // Move towards the assigned target
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the detected object matches the target
        if (collision.transform == target)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            Debug.Log("Target detected by the dragon!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Stop chasing if the target exits the detection area
        if (collision.transform == target)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            Debug.Log("Target escaped from the dragon!");
        }
    }
}
