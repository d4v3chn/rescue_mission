using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points for idle movement
    public float chaseSpeed = 5f; // Speed when chasing Gawe
    public float patrolSpeed = 2f; // Speed during patrol

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

        agent.speed = patrolSpeed; // Start with patrol speed
        GotoNextPatrolPoint();
    }

    void Update()
    {
        if (!agent.isOnNavMesh)
        {
            return; // Skip Update if the agent is not on the NavMesh
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
        if (!agent.isOnNavMesh || agent.pathPending)
            return;

        // If the dragon has reached its patrol point, go to the next one
        if (agent.remainingDistance < 0.5f)
        {
            GotoNextPatrolPoint();
        }
    }

    private void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        // Ensure patrol point is on NavMesh
        Transform target = patrolPoints[currentPatrolIndex];
        if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogWarning("Patrol point is not on the NavMesh.");
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void ChaseGawe()
    {
        // If chasing, move toward Gawe's position
        GameObject gawe = GameObject.FindGameObjectWithTag("Player");
        if (gawe != null && agent.isOnNavMesh)
        {
            agent.SetDestination(gawe.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Start chasing Gawe if detected
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            Debug.Log("Gawe detected by the dragon!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Stop chasing when Gawe is out of detection range
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            Debug.Log("Gawe escaped from the dragon!");
        }
    }
}
