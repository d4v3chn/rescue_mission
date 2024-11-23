using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catBehaviour : MonoBehaviour
{
       public float speed = 5f;  // Movement speed of the ghost
    public float detectionRange = 5f;  // Range within which the ghost tries to avoid the player
    public LayerMask wallLayer;  // Layer assigned to walls in the maze

    private Rigidbody2D rb;
    private Vector2 currentDirection;
    private Transform player;  // Reference to the player
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };  // Possible movement directions
    private bool isAvoidingPlayer = false;  // Track if ghost is actively avoiding the player

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Assume the player has a tag "Player"
        PickRandomDirection();  // Start with a random direction
    }

    void Update()
    {
        // Check if the ghost should avoid the player
        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            isAvoidingPlayer = true;
            AvoidPlayer();
        }
        else
        {
            isAvoidingPlayer = false;
        }

        // Ensure the ghost keeps moving if it stops or has no valid direction
        if (currentDirection == Vector2.zero || IsWallAhead())
        {
            PickRandomDirection();
        }
    }

    void FixedUpdate()
    {
        // Always move the ghost in the current direction
        rb.velocity = currentDirection.normalized * speed * Time.fixedDeltaTime;
    }

    // Picks a random valid direction for the ghost to move
    private void PickRandomDirection()
    {
        List<Vector2> validDirections = new List<Vector2>();

        foreach (var dir in directions)
        {
            if (!IsWallInDirection(dir))
            {
                validDirections.Add(dir);
            }
        }

        // Pick a random direction from the valid options
        if (validDirections.Count > 0)
        {
            currentDirection = validDirections[Random.Range(0, validDirections.Count)];
        }
        else
        {
            // If no valid directions are found, reverse direction
            currentDirection = -currentDirection; 
        }
    }

    // Checks if there's a wall ahead in the current direction
    private bool IsWallAhead()
    {
        return IsWallInDirection(currentDirection);
    }

    // Checks for a wall in a specific direction using a raycast
    private bool IsWallInDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
        return hit.collider != null;

    }

    // Logic to move the ghost away from the player
    private void AvoidPlayer()
    {
        Vector2 avoidanceDirection = Vector2.zero;
        float maxDistance = 0;

        foreach (var dir in directions)
        {
            if (!IsWallInDirection(dir))
            {
                float distance = Vector2.Distance((Vector2)transform.position + dir, player.position);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    avoidanceDirection = dir;
                }
            }
        }

        // If an avoidance direction is found, use it; otherwise, pick a random direction
        currentDirection = avoidanceDirection != Vector2.zero ? avoidanceDirection : currentDirection;

        // Fallback to ensure movement
        if (currentDirection == Vector2.zero)
        {
            PickRandomDirection();
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the ghost collides with something, pick a new random direction
        PickRandomDirection();

    if (collision.gameObject.CompareTag("Player"))
    {
        Destroy(gameObject); // Destroy the cat if caught by the player
    }
}

}
