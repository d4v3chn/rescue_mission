using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    enum Direction{
        North, South,East, West, None
    }
    public float speed;
    Rigidbody2D rb;
    private Direction movingDir = Direction.None;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    // Update is called once per frame
    void Update()
    {

        if(movingDir == Direction.None){
        if(Input.GetAxisRaw("Horizontal")!=0){

            if(Input.GetAxisRaw("Horizontal") > 0){
                movingDir = Direction.East;
            }
            else if(Input.GetAxisRaw("Horizontal") < 0){
                movingDir = Direction.West;
            }
        }
        else if(Input.GetAxisRaw("Vertical") != 0){

            if(Input.GetAxisRaw("Vertical") > 0){
                movingDir = Direction.North;
            }
            else if(Input.GetAxisRaw("Vertical") < 0){
                movingDir = Direction.South;
            }
        }
        }
    }

    void FixedUpdate(){
        switch(movingDir){
            case Direction.North:
                rb.velocity = new Vector2(0,speed*Time.fixedDeltaTime);
                break;
            case Direction.South:
                rb.velocity = new Vector2(0,-speed*Time.fixedDeltaTime);
                break;
            case Direction.East:
                rb.velocity = new Vector2(speed*Time.fixedDeltaTime,0);
                break;
            case Direction.West:
                rb.velocity = new Vector2(-speed*Time.fixedDeltaTime,0);
                break;
            case Direction.None:
                rb.velocity = Vector2.zero; // Stop if no direction is set
                break;
        }
    }




     void OnCollisionEnter2D(Collision2D collision)
    {
        // Reset movement only if the player hits something other than the cat
        if (!collision.gameObject.CompareTag("Cat"))
        {
            rb.velocity = Vector2.zero;
            movingDir = Direction.None; // Reset direction to allow new input
        }
    }
}
