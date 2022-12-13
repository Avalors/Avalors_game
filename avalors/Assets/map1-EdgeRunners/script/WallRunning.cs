using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform Orientation;
    [SerializeField] MovementController MC; 

    [Header("Detection")]
    [SerializeField] private float WallDistance = 0.6f;
    [SerializeField] private float MinimumJumpHeight = 1.5f;

    [Header("Wall Running")]
    //will test suitable values
    [SerializeField] private float WallRunGravity = 1f;
    [SerializeField] private float WallRunJumpForce = 5f;

    [Header("Camera")]
    [SerializeField] private Camera Cam;
    [SerializeField] private float WallRunFov;
    [SerializeField] private float WallRunFovTime;

    //camera tilt attributes/variables
    [SerializeField] private float CameraTilt;
    [SerializeField] private float CameraTiltTime;

    //this attribute will be accessible 
    public float Tilt { get; private set; }

    //Stores if there is a wall on the left and right
    bool WallLeft = false;
    bool WallRight = false;

    RaycastHit LeftWallHit;
    RaycastHit RightWallHit;

    private Rigidbody rb;


    public bool CanWallRun()
    {
        //returns the opposite bool value of if the raycast hits something
        return !Physics.Raycast(transform.position, Vector3.down, MinimumJumpHeight);
    }

    private void Start()
    {
        //gets component rigidbody from player object
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckWall();
        WallChecker();
    }
    
    void WallChecker()
    {
        if (CanWallRun())
        {
            //checks if there is a wall to the left 
            if (WallLeft)
            {
                StartWallRun();
            }
            //checks if there is a wall to the right
            else if (WallRight)
            {
                StartWallRun();
            }
            //stops wall running when there is no wall
            else
            {
                StopWallRun();
            }
        }
        //and if the player does not jump the Minimum Jump height
        else
        {
            StopWallRun();
        }
    }

    void CheckWall()
    {
        //if there is an object to the right and left of the orientation with a distance of 1.5 and return true
        //outputs results to raycast hit 
        WallLeft = Physics.Raycast(transform.position, -Orientation.right, out LeftWallHit, WallDistance);
        WallRight = Physics.Raycast(transform.position, Orientation.right, out RightWallHit, WallDistance);
    }

    void StartWallRun()
    {
        //disable gravity to prevent character from falling
        rb.useGravity = false;

        //Applies a downward gravity force of our choice
        rb.AddForce(Vector3.down * WallRunGravity, ForceMode.Force);

        //Lerping camera field of view to our wall run fov
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, WallRunFov, WallRunFovTime * Time.deltaTime);

        //if the wall is left we want the tilt to be towards the left
        if (WallLeft)
        {
            Tilt = Mathf.Lerp(Tilt, -CameraTilt, CameraTiltTime * Time.deltaTime);
        }
        else if (WallRight)
        {
            //by making the camera tilt positive we make the camera tilt the other way
            Tilt = Mathf.Lerp(Tilt, CameraTilt, CameraTiltTime * Time.deltaTime);
        }

        //checks if the player presses the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //checks if there is a wall to the left
            if (WallLeft)
            {
                //Creates a diagonal vector to use for wall running
                Vector3 WallRunJumpDirection = transform.up + LeftWallHit.normal;
                //reset player velocity on the y axis
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                //wall run movement force
                rb.AddForce(WallRunJumpDirection * WallRunJumpForce * 100, ForceMode.Force);
            }
            else if (WallRight)
            {
                //Creates a diagonal vector to use for wall running
                Vector3 WallRunJumpDirection = transform.up + RightWallHit.normal;
                //reset player velocity on the y axis
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                //wall run movement force
                rb.AddForce(WallRunJumpDirection * WallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }
    
    void StopWallRun()
    {
        //turns gravity back on when the player stops wall running
        rb.useGravity = true;

        //Lerping camera field of view to our wall run fov
        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, MC.Fov, WallRunFovTime * Time.deltaTime);

        //changes camera tilt to 0, which is the normal tilt|
        Tilt = Mathf.Lerp(Tilt, 0, CameraTiltTime * Time.deltaTime);
    }

}
