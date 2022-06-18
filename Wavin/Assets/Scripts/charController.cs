using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{
    private float x, y;
    private float x_input;
    private float xMomentum;
    public float maxX;
    private bool grounded;
    public float gravity;
    public float airDrift;
    public float groundSpeed;
    public float jumpSpeed;
    public GameObject ParentObject;
    private bool facingDir;
    // Start is called before the first frame update
    void Start()
    {
        x = 0f;
        y = 0f;
        grounded = false;
        facingDir = true;
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Ground" && grounded == false){
            y = 0;
            grounded = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // x movement on ground
        if(grounded == true){
            x_input = Input.GetAxisRaw("Horizontal")*groundSpeed;
            if(facingDir && x<0){
                ParentObject.transform.localScale = new Vector3(-1,1,1);
                facingDir = false;
            }
            else if(!facingDir && x>0){
                ParentObject.transform.localScale = new Vector3(1,1,1);
                facingDir = true;
            }
            if(x_input!=0){
                x = x_input; // Snappy movement
            }
            else{
                if(x!=0){
                    x = Mathf.Lerp(x, 0, 0.015f); // Some inertia on release
                }
            }
            ParentObject.transform.position = ParentObject.transform.position + new Vector3(x, 0, 0)*Time.deltaTime;
        }
        else{ // x movement on air
            xMomentum = Input.GetAxis("Horizontal")*airDrift*Time.deltaTime;
            x += xMomentum;
            if(Mathf.Abs(x)>maxX){
                x = (x>0) ? maxX : -maxX;
            }
            // update x position
            ParentObject.transform.position = ParentObject.transform.position + new Vector3(x, 0, 0)*Time.deltaTime;
        }

        // y movement
        if(Input.GetAxis("Vertical") > 0 && grounded == true){
            y = jumpSpeed;
            grounded = false;
        }
        if(grounded == false){
            y += gravity * Time.deltaTime;
        }
        // update y position (does not work on slopes)
        ParentObject.transform.position = ParentObject.transform.position + new Vector3(0, y, 0)*Time.deltaTime;
    }
}
