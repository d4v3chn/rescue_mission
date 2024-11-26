using UnityEngine;

public class Gawe : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;

    public LayerMask obstacleLayer;
    public LayerMask checkpointLayer;

    private new Rigidbody2D rigidbody;
    private Vector2 direction;
    private Vector2 nextDirection;
    private Vector3 startingPosition;
    private Vector3 currentCheckpoint;
    private bool isAtCheckpoint = false;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        currentCheckpoint = startingPosition;
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        HandleInput();

        Collider2D checkpointCollider = Physics2D.OverlapBox(transform.position, Vector2.one * 0.475f, 0f, checkpointLayer);
        isAtCheckpoint = checkpointCollider != null;

        if (isAtCheckpoint)
        {
            currentCheckpoint = checkpointCollider.transform.position;
        }

        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
        else if (Vector2.Distance(transform.position, currentCheckpoint) < 0.1f)
        {
            FindNextCheckpoint();
        }
    }

    private void FindNextCheckpoint()
    {
        Collider2D[] checkpoints = Physics2D.OverlapBoxAll(transform.position, Vector2.one * 1.5f, 0f, checkpointLayer);
        if (checkpoints.Length > 0)
        {
            currentCheckpoint = checkpoints[0].transform.position;
            SetDirection((currentCheckpoint - (Vector3)transform.position).normalized);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetDirection(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetDirection(Vector2.left);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * speed * speedMultiplier * Time.fixedDeltaTime;
        rigidbody.MovePosition(position + translation);
    }

    public void ResetState()
    {
        speedMultiplier = 1.0f;
        direction = Vector2.zero;
        nextDirection = Vector2.zero;
        transform.position = currentCheckpoint;
        rigidbody.isKinematic = false;
        enabled = true;
    }

    public void SetDirection(Vector2 newDirection)
    {
        if (!Occupied(newDirection))
        {
            direction = newDirection;
            nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = newDirection;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.475f, 0.0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }
}