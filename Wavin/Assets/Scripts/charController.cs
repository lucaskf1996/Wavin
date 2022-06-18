using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{
    private float x, y;
    private float x_input;
    private float xMomentum;
    private float tmpAirSpeed;

    public float raySize;
    public float maxX;
    public float gravity;
    public float airDrift;
    public float groundSpeed;
    public float jumpSpeed;
    public float airDodgeSpeed;
    public float airDodgeDecelerate;
    public float groundInertia;
    public float landInertia;

    private bool grounded;
    private bool facingDir;
    private bool airDodged;
    private bool landed;
    private bool canMoveInAir;

    public GameObject ParentObject;
    public GameObject RaycastPoint;
    
    private Vector3 airDodgeDir;
    // Start is called before the first frame update
    void Start()
    {
        x = 0f;
        y = 10f;
        grounded = false;
        facingDir = true;
        airDodged = false;
        landed = false;
        canMoveInAir = true;
    }

    void OnCollisionEnter2D(Collision2D collision){
        RaycastHit2D hit = Physics2D.Raycast(RaycastPoint.transform.position, transform.TransformDirection(Vector2.down), raySize);
        if(hit){
            if(collision.gameObject.tag == "Ground" && grounded == false){
                y = 0;
                grounded = true;
                if(airDodged){
                    landed = true;
                }
                airDodged = false;
                canMoveInAir = true;
                Debug.Log(airDodged);
            }
        }
    }
    void OnCollisionStay2D(Collision2D collision){
        RaycastHit2D hit = Physics2D.Raycast(RaycastPoint.transform.position, transform.TransformDirection(Vector2.down), raySize);
        if(hit){
            if(collision.gameObject.tag == "goup" && grounded == true){
                ParentObject.transform.position += new Vector3(0, 2f, 0)*Time.deltaTime;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // x movement on ground
        if(grounded == true){
            if(!landed){
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
                        x = Mathf.Lerp(x, 0, groundInertia); // Some inertia on release 0.015f
                    }
                }
            }
            else{
                x = Mathf.Lerp(x, 0, landInertia);
                if(Mathf.Abs(x) < 2.7f){
                    landed = false;
                }
            }
        }
        else{ // x movement on air
            if(canMoveInAir){
                xMomentum = Input.GetAxis("Horizontal")*airDrift*Time.deltaTime;
                x += xMomentum;
                if(Mathf.Abs(x)>maxX){
                    x = (x>0) ? maxX : -maxX;
                }
            }
            else{
                tmpAirSpeed = Mathf.Lerp(tmpAirSpeed, 0, airDodgeDecelerate);
                if(Mathf.Abs(tmpAirSpeed) < 0.5f){
                    canMoveInAir = true;
                }
                x = airDodgeDir.x*tmpAirSpeed;
                y = airDodgeDir.y*tmpAirSpeed;
            }
        }

        // Jump
        if(canMoveInAir){
            if(Input.GetButtonDown("Jump") && grounded == true){
                y = jumpSpeed;
                grounded = false;
            }
            if(grounded == false){
                y += gravity * Time.deltaTime;
            }
        }

        if(!airDodged && !grounded && Input.GetButtonDown("Fire1")){
            airDodged = true;
            canMoveInAir = false;
            airDodgeDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            airDodgeDir.Normalize();
            tmpAirSpeed = airDodgeSpeed;
        }

        // update y position (does not work on slopes)
        ParentObject.transform.position += new Vector3(0, y, 0)*Time.deltaTime;
        // update x position
        ParentObject.transform.position += new Vector3(x, 0, 0)*Time.deltaTime;

        rayHit();
    }

    void rayHit(){
        RaycastHit2D hit = Physics2D.Raycast(RaycastPoint.transform.position, transform.TransformDirection(Vector2.down), raySize);
        if(!hit){
            grounded = false;
        }
    }
}
