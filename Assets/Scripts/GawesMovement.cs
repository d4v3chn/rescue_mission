using UnityEngine;

public class GaweCheckpointMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public LayerMask checkpointLayer; // LayerMask for checkpoints
    private Vector2 targetPosition; // Target position for movement
    private bool isMoving = false; // Is Gawe moving?
    private Rigidbody2D rb; // Rigidbody for movement

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SnapToGrid(); // Ensure Gawe starts on a grid-aligned position
    }

    void Update()
    {
        // If not moving, allow input
        if (!isMoving)
        {
            Vector2 inputDirection = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                inputDirection = Vector2.up;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                inputDirection = Vector2.down;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                inputDirection = Vector2.right;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                inputDirection = Vector2.left;

            if (inputDirection != Vector2.zero)
            {
                AttemptMove(inputDirection);
            }
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            // Move towards the target position
            Vector2 currentPosition = rb.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // Check if Gawe has reached the target
            if (Vector2.Distance(newPosition, targetPosition) < 0.05f)
            {
                StopMovement();
            }
        }
    }

    void AttemptMove(Vector2 direction)
    {
        // Cast a ray to find the next checkpoint
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, checkpointLayer);

        if (hit.collider != null)
        {
            // Set the target position to the checkpoint's position
            targetPosition = hit.collider.transform.position;
            isMoving = true;
        }
        else
        {
            Debug.Log("No checkpoint in that direction.");
        }
    }

    void StopMovement()
    {
        isMoving = false;
        rb.velocity = Vector2.zero;
        SnapToGrid(); // Ensure Gawe stops precisely on the grid
    }

    void SnapToGrid()
    {
        Vector2 currentPosition = transform.position;
        Vector2 snappedPosition = new Vector2(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y));
        rb.position = snappedPosition;
    }
}
