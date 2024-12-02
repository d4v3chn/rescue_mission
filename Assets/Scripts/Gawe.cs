using UnityEngine;

public class Gawe : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float tunnelSpeed = 2f;
    public Rigidbody2D rb;
    public GameManager GM;

    private float currentSpeed;
    private Vector2 movement;
    private bool isFacingRight = true;

    void Start()
    {
        ResetState();
    }

    private void Awake()
    {
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (movement.x < 0 && !isFacingRight)
        {
            Flip(); 
        }
        else if (movement.x > 0 && isFacingRight)
        {
            Flip(); 
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tunnel"))
        {
            currentSpeed = tunnelSpeed;

            Debug.Log("Gawe entered the tunnel, the speed reduced...");
        }
        // Dragon catches Gawe and the game resets
        if (collision.CompareTag("CatchGawe"))
        {
            Debug.Log("Gawe caught by the dragon!");

            if (GM != null)
            {
                GM.Death();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tunnel"))
        {
            currentSpeed = normalSpeed;

            Debug.Log("Gawe exited the tunnel, the speed is back to normal!");
        }
    }

    private void Flip()
    {

        isFacingRight = !isFacingRight;
        
        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale;
    }

    public void ResetState(){
        rb.position = new Vector3(0.5f, -10.59f, 0f);
        enabled = true;
    }
}
