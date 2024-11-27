 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // For NavMesh functionality

public class CatBehaviour : MonoBehaviour
{
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


        // Initial direction for patrol (we can start in any direction)
        lastDirection = transform.right; // Initially move to the right
    }

    private void Update()
    {
        if (agent.isOnNavMesh && chaser != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, chaser.position);

            if (distanceToPlayer <= safeDistance)
            {
                // Move away from the player
                Vector3 awayDirection = (transform.position - chaser.position).normalized;
                Vector3 runToPosition = transform.position + awayDirection * runDistance;
                runToPosition.z = transform.position.z; // Ensure movement is 2D
                agent.SetDestination(runToPosition);
            }
            else
            {
                Patrol(); // Start patrolling when player is far away
            }
        }
    }


    /*Things below this should woirk*/

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
            randomPosition = new Vector3(randomX * gridSizeX + 0.5f, randomY * gridSizeY + 0.5f, 0f);

            // Check if the chosen position is valid (not on a wall) using NavMesh
            NavMeshHit hit;
            validPosition = NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas);
        }

        // Set the cat's position to the valid position
        transform.position = randomPosition;
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