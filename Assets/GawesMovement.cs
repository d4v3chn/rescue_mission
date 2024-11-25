using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaweMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public LayerMask obstacleLayer; // LayerMask for walls and checkpoints
    public Rigidbody2D rb;
    private Vector2 moveDirection; // Direction of movement
    private Vector2 targetPosition; // Target grid position
    private bool isMoving = false; // Is Gawe currently moving?

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SnapToGrid(); // Ensure starting position aligns with grid
    }

    private void Update()
    {
        if (!isMoving) // Only take input if not moving
        {
            // Check for input
            if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector2.down;
            else if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector2.left;
            else if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector2.right;

            if (moveDirection != Vector2.zero)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 1f, obstacleLayer);
                if (hit.collider == null) // No obstacle in the way
                {
                    targetPosition = (Vector2)transform.position + moveDirection;
                    isMoving = true;
                }
                else
                {
                    Debug.Log($"Blocked by: {hit.collider.name}");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb.velocity = moveDirection * speed;

            // Check if we've reached the target position
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition; // Snap to the exact target
                StopMovement();
            }
        }
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        isMoving = false;
    }

    private void SnapToGrid()
    {
        Vector2 snappedPosition = new Vector2(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y)
        );
        transform.position = snappedPosition;
        Debug.Log($"Snapped Position: {snappedPosition}");
    }
}
