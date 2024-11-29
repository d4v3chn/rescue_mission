using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;

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

        if (agent.remainingDistance < 0.5f)
        {
            GotoNextPatrolPoint();
        }
    }

    private void GotoNextPatrolPoint()
    {
        if (patrolPoints.Length == 0)
            return;

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
