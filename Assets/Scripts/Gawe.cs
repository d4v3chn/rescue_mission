using UnityEngine;

public class Gawe : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float tunnelSpeed = 2f;

    private float currentSpeed;
    public Rigidbody2D rb; 

    private Vector2 movement;
    private bool isFacingRight = true;

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
}
