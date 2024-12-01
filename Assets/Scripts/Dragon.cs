using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of patrol points for patrol movement
    public Transform gawe;           // Reference to Gawe (drag and drop in Inspector)
    public float chaseSpeed = 5f;    // Speed when chasing Gawe
    public float patrolSpeed = 2f;   // Speed during patrol
    public float originalRadius = 5f; // Original detection radius
    public float increasedRadius = 8f; // Increased radius when chasing

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private CircleCollider2D detectionCollider;

    private void Awake()
    {
        // Initialize the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the Dragon GameObject.");
            return;
        }

        // Set NavMeshAgent properties for 2D alignment
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Initialize the CircleCollider2D
        detectionCollider = GetComponent<CircleCollider2D>();
        if (detectionCollider == null)
        {
            Debug.LogError("CircleCollider2D is missing on the Dragon GameObject.");
            return;
        }

        // Set the initial radius
        detectionCollider.radius = originalRadius;
    }

    void Start()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh or is missing.");
            return;
        }

        // Set initial patrol speed and start patrolling
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
        // Check if agent is idle and not heading to a patrol point
        if (agent.pathPending || agent.remainingDistance > 0.5f)
            return;

        // Go to the next patrol point
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

        // Set the destination to Gawe's position
        agent.SetDestination(gawe.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == gawe)
        {
            isChasing = true;
            agent.speed = chaseSpeed;

            // Increase the detection radius
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

            // Reset the detection radius
            detectionCollider.radius = originalRadius;

            // Restart patrolling
            ResumePatrol();

            Debug.Log("Gawe escaped! Dragon is returning to patrol.");
        }
    }

    private void ResumePatrol()
    {
        // Ensure the dragon moves to the nearest patrol point after chasing ends
        GotoNextPatrolPoint();
    }
}
