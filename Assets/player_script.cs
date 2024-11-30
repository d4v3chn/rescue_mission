using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_script : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;
    bool movingHorizontally, canCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(movingHorizontally){
            if(Physics2D.Raycast(transform.position, Vector2.left)){
            canCheck = true;
            }
            else{
            canCheck = false;
            }
        }

        if(canCheck){
            if(Input.GetAxisRaw("Horizontal") != 0){

            }
        }
    }

}
