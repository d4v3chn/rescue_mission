using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points for idle movement
    public float chaseSpeed = 5f;    // Speed when chasing Gawe
    public float patrolSpeed = 2f;   // Speed during patrol

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    private void Awake()
    {
        // Initialize the agent in Awake
        agent = GetComponent<NavMeshAgent>();

        // Check if agent exists to prevent NullReferenceException
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the Dragon GameObject.");
            return;
        }

        // Set NavMeshAgent properties
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Start()
    {
        // Check if the agent is valid before proceeding
        if (agent == null) return;

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
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh or is missing.");
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
        if (agent == null || !agent.isOnNavMesh || agent.pathPending)
            return;

        // If the dragon has reached the current patrol point, move to the next one
        if (agent.remainingDistance < 0.5f)
        {
            GotoNextPatrolPoint();
        }
    }

    private void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0 || agent == null) return;

        // Ensure patrol point is on NavMesh
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
        if (agent == null) return;

        // If chasing, move toward Gawe's position
        GameObject gawe = GameObject.FindGameObjectWithTag("Player");
        if (gawe != null && agent.isOnNavMesh)
        {
            agent.SetDestination(gawe.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            Debug.Log("Gawe detected by the dragon!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            Debug.Log("Gawe escaped from the dragon!");
        }
    }
}
