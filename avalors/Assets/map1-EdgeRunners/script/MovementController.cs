using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    float PlayerHeight = 2f;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    //default movement speed  
    public float MovementSpeed = 6f;
    //will speed up the movement speed
    public float MovementMultiplier = 10f;
    //will slow down the air movement to reasonable speed
    [SerializeField] float AirMultiplier = 0.5f;

    [Header("Sprinting")]
    //different adjustable speeds for walking, sprinting and accellerating between the two speeds
    [SerializeField] float WalkSpeed = 4f;
    [SerializeField] float SprintSpeed = 6f;
    [SerializeField] float Accelleration = 10f;

    [Header("Jumping")]
    //default jump force (will test and change)
    public float JumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode SprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    //Ground and air drag values
    public float GroundDrag = 6f;
    public float AirDrag = 1f;

    //stores Horizontal and Vertical movement
    float HorizontalMovement;
    float VerticalMovement;

    [Header("Ground Detection")]
    //Will be my position of my checksphere
    [SerializeField] Transform GroundCheck;
    //Ground Layer
    [SerializeField] LayerMask GroundMask;
    //variable that checks if the player has hit the ground
    bool IsGrounded;
    float GroundDistance = 0.4f;

    //reference to my rigidbody character
    private Rigidbody rb;

    //stores the characters movement direction
    Vector3 MovementDirection;
    Vector3 SlopeMovementDirection;

    //stores Raycast informatoin
    RaycastHit SlopeHit;
    
    //checks if we are on a slope
    private bool OnSlope()
    {
        //outputs if the slope is hit to SlopeHit variable
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, PlayerHeight / 2 + 0.5f))
        {
            //checks if its a slope as the normal ray would not hit the ground perpendiularly 
            if (SlopeHit.normal != Vector3.down)
            {
                //return true indicates that the player is on a slope
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        //gets rigidbody component from player object and saves it to defined variable
        rb = GetComponent<Rigidbody>();

        //keeps the rotation the same
        rb.freezeRotation = true; 
    }

    //embedded methods are called each frame
    private void Update() 
    {
        MyInput(); 
        PlayerDrag();
        ControlSpeed();
        CheckGround();
        Jump();
        SlopeDirection();
    }

    void MyInput()
    {
        // checks player input of w,a,s,d and return numerical value of 1 or -1 for x and y direction and saves to variable
        HorizontalMovement = Input.GetAxisRaw("Horizontal"); 
        VerticalMovement = Input.GetAxisRaw("Vertical"); 

        //establishes movement direction taking into consideration where the player is moving and the keys he/she presses
        //Now moves according to the orientation
        MovementDirection = orientation.forward * VerticalMovement + orientation.right * HorizontalMovement; 
    }

    void PlayerDrag()
    {
        //selection statement changes the drag value depending on if the player is in the air or not
        if (IsGrounded)
        {
            rb.drag = GroundDrag;
        }
        else
        {
            rb.drag = AirDrag;
        }
    }

    void ControlSpeed()
    {
        if(Input.GetKey(SprintKey) && IsGrounded)
        {
            //changes the orignal speed to the sprint speed and applies the acceleration for how long the player holds the SprintKey
            MovementSpeed = Mathf.Lerp(MovementSpeed, SprintSpeed, Accelleration * Time.deltaTime);
        }
        else
        {
            //changes the original speed to teh walk speed and decellerates appropriately
            MovementSpeed = Mathf.Lerp(MovementSpeed, WalkSpeed, Accelleration * Time.deltaTime);
        }
    }

    void CheckGround()
    {
        //checks if the player is touching the ground by determining the players position by subtracting from it a vector of half the size of the capsule.
        //using transform Ground Check simplifies the code 
        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
    }

    void Jump()
    {
        //selection statement checks if JumpKey is true (which is when space bar is pressed) and player is grounded
        if (Input.GetKeyDown(JumpKey) && IsGrounded)
        {
            //resets y velocity to 0, when the player hits the ground
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            //Adds a sudden force 
            rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
    }

    void SlopeDirection()
    {
        //changes direction to a direction that is parallel to the slope
        SlopeMovementDirection = Vector3.ProjectOnPlane(MovementDirection, SlopeHit.normal);
    }

    //has the same freqeuncy as the physics system
    private void FixedUpdate()
    { 
        MovementPlayer();
    }

    void MovementPlayer()
    {
        if (IsGrounded && !OnSlope())
        {
            //causes player to accellerate in the correct direction
            rb.AddForce(MovementDirection.normalized * MovementSpeed * MovementMultiplier, ForceMode.Acceleration);
            //.normalized ensures the movement directions magnitude is equal on all sides, including the diagonals
        }
        //checks if player is touching the ground and is on a slope to accellerate in the direction parallel to the slope
        else if (IsGrounded && OnSlope()) 
        {
            rb.AddForce(SlopeMovementDirection.normalized * MovementSpeed * MovementMultiplier, ForceMode.Acceleration);
        }
        else if (!IsGrounded)
        {
            //AirMultiplier is multiplied which will slow down the speed in the air to a reasonable ammount
            rb.AddForce(MovementDirection.normalized * MovementSpeed * MovementMultiplier * AirMultiplier, ForceMode.Acceleration);
        }
    }
}
