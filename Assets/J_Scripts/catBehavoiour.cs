using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl; // For NavMesh functionality

public class CatBehaviour : MonoBehaviour
{
    /* Cat AI States */
    private enum CatState { Patrolling, RunningAway, Caught }
    private CatState currentState;

    public GameManager GM;
    /*To randomly select a startpoint*/
    private float gridSizeX = 1f; // Width of a grid square
    private float gridSizeY = 1f; // Height of a grid square
    private int gridWidth = 22; // Width of the grid (22 squares)
    private int gridHeight = 22; // Height of the grid (22 squares)

    /*The agent = the cat*/
    NavMeshAgent agent;
    [SerializeField] float runSpeed = 10f; // Distance at which the cat switches to patrol mode
    [SerializeField] float patrolSpeed = 10f; // Distance at which the cat switches to patrol mode


    /*The chaser = the player*/
    [SerializeField] Transform chaser; // Player transform
    [SerializeField] float runDistance = 5f; // Distance to move away from the player
    [SerializeField] float safeDistance = 10f; // Distance at which the cat switches to patrol mode

    /*Variables important for the patrol state */
    private float patrolTime = 0.5f; // Time to move in one direction
    private float timeInCurrentDirection = 0f; // Timer for how long we've been moving in the current direction
    private Vector3 lastDirection; // The direction the cat is moving in (for reversal)


    void Start()
    {
        // Start at a random position
        SetRandomStartPosition();
        //movePoint.parent = null;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Prevent rotation for 2D
        agent.updateUpAxis = false;   // Align with 2D plane

        // Initial state
        currentState = CatState.Patrolling;

        // Initial direction for patrol (we can start in any direction)
        lastDirection = transform.right; // Initially move to the right
    }

    private void Update()
    {
        switch (currentState)
        {
            case CatState.Patrolling:
                agent.speed = patrolSpeed;
                Patrol();
                CheckPlayerDistance();
                LockRotation();
                break;

            case CatState.RunningAway:
                agent.speed = runSpeed;
                RunAwayFromPlayer();
                CheckPlayerDistance();
                LockRotation();
                break;

            case CatState.Caught:
                Debug.Log("Cat caught!");
                // No movement or behavior; the cat is "caught."
                GM.SetScore(GM.score + 1);
                GM.ResetEntities();
                break;
        }
    }

    private void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, chaser.position);

        if (distanceToPlayer <= safeDistance)
        {
            currentState = CatState.RunningAway;
        }
        else if (distanceToPlayer > safeDistance && currentState == CatState.RunningAway)
        {
            currentState = CatState.Patrolling;
        }
    }

    // Trigger collision detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentState != CatState.Caught) // Check if the player caught the cat
        {
            currentState = CatState.Caught;
        }
    }

    // Respawn the cat at a random location
    public void Respawn()
    {
        currentState = CatState.Patrolling; // Reset to patrolling
        SetRandomStartPosition(); // Randomly place the cat
    }

    /*Function to decide where the cat sould spawn*/
    void SetRandomStartPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false;

        // Keep trying until a valid position is found
        while (!validPosition)
        {
            // Randomly select a grid cell within the grid limits
            float randomX = Random.Range(-gridWidth/2, gridWidth/2);  // Random X coordinate (between 0 and gridWidth-1)
            float randomY = Random.Range(-gridHeight/2, gridHeight/2); // Random Y coordinate (between 0 and gridHeight-1)

            // Calculate the center of the random grid square (0.5 offset to center in the square)
            randomPosition = new Vector3(randomX * gridSizeX - 0.5f, randomY * gridSizeY + 0.2f, 0f);

            // Check if the chosen position is valid (not on a wall) using NavMesh
            NavMeshHit hit;
            validPosition = NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas);
        }

        // Set the cat's position to the valid position
        transform.position = randomPosition;
    }

    private void RunAwayFromPlayer()
    {
        Vector3 awayDirection = (transform.position - chaser.position).normalized;
        Vector3 runToPosition = transform.position + awayDirection * runDistance;
        runToPosition.z = transform.position.z; // Ensure movement is 2D
        agent.SetDestination(runToPosition);
        Debug.Log($"Running away to: {runToPosition}");
    }

    private void Patrol()
    {
        if (timeInCurrentDirection >= patrolTime)
        {
            lastDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
            timeInCurrentDirection = 0f;
        }

        timeInCurrentDirection += Time.deltaTime;
        agent.SetDestination(transform.position + lastDirection);
        Debug.Log($"Patrolling towards: {agent.destination}");
    }
    private void LockRotation()
{
    // Keep the rotation of the cat locked on the Z-axis (0 rotation)
    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
}
}