
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

    /*The chaser = the player*/
    [SerializeField] Transform chaser; // Player transform
    [SerializeField] float runDistance = 5f; // Distance to move away from the player
    [SerializeField] float safeDistance = 10f; // Distance at which the cat switches to patrol mode
    //[SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private Transform movePoint;
    //[SerializeField] List<Transform> patrolPoints; // Points for patrolling
    //private int currentPatrolIndex = 0;

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
                        Patrol();
                        CheckPlayerDistance();
                        break;

                    case CatState.RunningAway:
                        RunAwayFromPlayer();
                        CheckPlayerDistance();
                        break;

                    case CatState.Caught:
                    Debug.Log("Cat caught!");
                        // No movement or behavior; the cat is "caught."
                        GM.SetScore(GM.score + 1);
                        Debug.Log(GM.score);
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


    /*Things below this should work*/

    /*Function to decide where the cat sould spawn*/
     void SetRandomStartPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        bool validPosition = false;

        // Keep trying until a valid position is found
        while (!validPosition)
        {
            // Randomly select a grid cell within the grid limits
            float randomX = Random.Range(0, gridWidth);  // Random X coordinate (between 0 and gridWidth-1)
            float randomY = Random.Range(0, gridHeight); // Random Y coordinate (between 0 and gridHeight-1)

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
    }

    private void Patrol()
    {
                // If we're moving in a direction, increment the timer
        timeInCurrentDirection += Time.deltaTime;

        // If we've been moving in this direction for the specified time, reverse direction
        if (timeInCurrentDirection >= patrolTime)
        {
            lastDirection = -lastDirection; // Reverse the patrol direction
            timeInCurrentDirection = 0f; // Reset the timer
        }

        // Set the agent's destination to move in the current direction for the next patrolTime
        agent.SetDestination(transform.position + lastDirection);

        // Optionally, adjust speed or other properties for patrol
        agent.speed = 0.5f; // Set a slow patrol speed
    }
}