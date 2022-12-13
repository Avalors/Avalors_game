using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    float PlayerHeight = 2f;

    [SerializeField] Transform orientation;

    [Header("References")]
    //connects wall running script to allow me to borrow fov public attribute
    [SerializeField] WallRunning WallRun;

    [Header("Camera")]
    //camera attributes
    [SerializeField] private Camera Cam;
    public float Fov = 90f;
    [SerializeField] private float SprintRunFov = 100f;
    [SerializeField] private float SprintFovTime = 20f;

    [Header("Movement")]
    //default movement speed  
    public float MovementSpeed = 6f;
    //will speed up the movement speed
    public float MovementMultiplier = 10f;
    //will slow down the air movement to reasonable speed
    [SerializeField] private float AirMultiplier = 0.5f;
    //will store the changing speed
    float DesiredSpeed;

    //public float for speed, which i will use to present in user Hud
    public float RBSpeed;

    [Header("Sprinting")]
    //different adjustable speeds for walking, sprinting and accellerating between the two speeds
    [SerializeField] private float WalkSpeed = 5f;
    [SerializeField] private float SprintSpeed = 10f;
    [SerializeField] private float Accelleration = 10f;

    [Header("Crouching")]
    //crouching attributes
    [SerializeField] private float CrouchSpeed = 2f;
    [SerializeField] private float CrouchYScale = 0.5f;
    [SerializeField] private float CrouchMultiplier;
    private float StartYScale;

    [Header("Sliding")]
    //sliding attributes
    [SerializeField] private float MaximumSlideTime = 0.5f;
    [SerializeField] private float SlideForce = 75f;
    [SerializeField] private float MaximumSlideSpeed = 30f;

    private float SlideTimer;

    //used to check player is sliding
    private bool sliding;

    [Header("Jumping")]
    //default jump force (will test and change)
    public float JumpForce = 10f;
    public float DoubleJumpForce = 5f;

    //check if player has already jumped twice
    private bool CanDoubleJump = false;

    [Header("Keybinds")]
    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode SprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode CrouchKey = KeyCode.LeftControl;

    [Header("Drag")]
    //Ground and air drag values
    [SerializeField] private float GroundDrag = 6f;
    [SerializeField] private float AirDrag = 1f;
    [SerializeField] private float SlidingAirDrag;

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

    //stores the angle of the slope
    float SlopeAngle;
    
    //checks if we are on a slope
    private bool OnSlope()
    {
        //outputs if the slope is hit to SlopeHit variable
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, PlayerHeight / 2 + 0.5f))
        {
            //gets slope angle 
            SlopeAngle = Vector3.Angle(Vector3.up, SlopeHit.normal);

            //checks if its a slope as the normal ray would not hit the ground perpendiularly 
            if (SlopeHit.normal != Vector3.down && SlopeAngle > 0.01f)
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

    void Mommentum()
    {
        float difference = Mathf.Abs(MovementSpeed - DesiredSpeed);
        float time = 0;

        if (difference > 4)
        {
            while (time < difference)
            {
                //will decrease the movement speed to the walk speed till maximum slide speed is reached
                MovementSpeed = Mathf.Lerp(MovementSpeed, DesiredSpeed, time / difference);

                //increments time per second
                time += Time.deltaTime;
            }
        }
        else
        { 
            MovementSpeed = Mathf.Lerp(MovementSpeed, DesiredSpeed, Accelleration * Time.deltaTime);
        }
    }

    private void Start()
    {
        //gets rigidbody component from player object and saves it to defined variable
        rb = GetComponent<Rigidbody>();

        //keeps the rotation the same
        rb.freezeRotation = true;

        //Saving normal y scale of the player
        StartYScale = transform.localScale.y;
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
        Crouch();
        CanSlide();
        Velocity();
    }

    void Velocity()
    {
        //stores vector of the velocity
        var RBVelocity = rb.velocity;

        //converts vector into magnitiude form to 2d.p
        RBSpeed = Mathf.Round(RBVelocity.magnitude * 100f) / 100f;
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

    void Crouch()
    {
        //checks if crouch key is held down
        if (Input.GetKey(CrouchKey))
        {
            //changes y scale to crouch scale
            transform.localScale = new Vector3(transform.localScale.x, CrouchYScale, transform.localScale.z);

            //downwards impulse
            rb.AddForce(Vector3.down * CrouchMultiplier, ForceMode.Impulse);

            //changes movement speed to slower crouch speed
            MovementSpeed = Mathf.Lerp(MovementSpeed, CrouchSpeed, Accelleration * Time.deltaTime);

        }
        //when the crouch button is not pressed stops crouching
        else
        {
            //reset player scale back to normal
            transform.localScale = new Vector3(transform.localScale.x, StartYScale, transform.localScale.z);

            //sets movement speed back to walking speed
            MovementSpeed = Mathf.Lerp(MovementSpeed, WalkSpeed, Accelleration * Time.deltaTime);
        }
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

            //increases the drag in the air if the player decides to slide
            if (sliding)
            {
                rb.drag = SlidingAirDrag;
            }
        }
    }

    void ControlSpeed()
    {
        if(Input.GetKey(SprintKey) && IsGrounded)
        {
            DesiredSpeed = SprintSpeed;

            Mommentum();

            //Lerping camera field of view to our Sprint run fov
            Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, SprintRunFov, SprintFovTime * Time.deltaTime);
        }
        //if the player is not wall running and is not sprinting the fov is returned to normal
        else 
        {
            //sets the input to the mommentum function to walk speed
            DesiredSpeed = WalkSpeed;

            Mommentum();

            //Lerping camera field of view to our normal fov
            Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, Fov, SprintFovTime * Time.deltaTime);
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
            //sets can double jump to true, to allow the player to double jump
            CanDoubleJump = true;

            //resets y velocity to 0, when the player hits the ground
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            //Adds a sudden force 
            rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
        //checks if the jump key is pressed and the player has not double jumped already
        else if (Input.GetKeyDown(JumpKey) && (CanDoubleJump))
        {
            //sets can double jump to false so that the player cannot jump again
            CanDoubleJump = false;

            //Adds a sudden force (allows the player to jump again)
            rb.AddForce(transform.up * DoubleJumpForce, ForceMode.Impulse);
        }
    }

    void SlopeDirection()
    {
        //changes direction to a direction that is parallel to the slope
        SlopeMovementDirection = Vector3.ProjectOnPlane(MovementDirection, SlopeHit.normal);
    }

    private bool SlopeUpOrDown()
    {
        if (OnSlope() && Physics.Raycast(transform.position, MovementDirection, 10f))
        {
            return false;
        }
        else if (OnSlope() && !Physics.Raycast(transform.position, MovementDirection, 10f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CanSlide()
    {
        //returns true as long as player is moving and slide key is pressed
        if (Input.GetKeyDown(CrouchKey) && (HorizontalMovement != 0 || VerticalMovement != 0))
        {
            StartSlide();
        }
        if (Input.GetKeyUp(CrouchKey) && sliding)
        {
            StopSlide();
        }
    }

    void StartSlide()
    {
        //sets sliding to true for CanSlide()
        sliding = true;

        //pushes player back down to the ground
        rb.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);

        //changes slide timer to maximum slide time
        SlideTimer = MaximumSlideTime;

    }

    void StopSlide()
    {
        sliding = false;

        //resets the player scale back to normalD
        transform.localScale = new Vector3(transform.localScale.x, StartYScale, transform.localScale.z);
    }

    //has the same freqeuncy as the physics system
    private void FixedUpdate()
    { 
        MovementPlayer();

        //if the player is sliding call the movement function is called
        if (sliding)
        {
            SlidingMovement();
        }
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

    private void SlidingMovement()
    {
        //will return true if the player is on a slope, however going up the slope and is on the ground
        if (!OnSlope())
        {

            //Applies slide force in the direction parallel to the ground
            rb.AddForce(MovementDirection.normalized * SlideForce, ForceMode.Force);

            //reduces the slide timer each second
            SlideTimer -= Time.deltaTime;

            //once the timer runs out the stopSldie method is executed
            if (SlideTimer <= 0)
            {
                StopSlide();
            }

        }
        //the raycast determines if the player is going up the slope, by creating a ray in the direction the player is moving
        else if (OnSlope())
        {
            if (!SlopeUpOrDown())
            {
                //Applies slide force in direction parallel to the slope
                rb.AddForce(SlopeMovementDirection.normalized * SlideForce, ForceMode.Force);

                //reduces the slide timer each second
                SlideTimer -= Time.deltaTime;

                //once the timer runs out the stopSldie method is executed
                if (SlideTimer <= 0)
                {
                    StopSlide();
                }
            }
            else if (SlopeUpOrDown())
            {
                //Applies slide force in direction parallel to the slope
                rb.AddForce(SlopeMovementDirection.normalized * SlideForce, ForceMode.Force);

                //sets Desired speed to the max slide speed as input for the Mommentum method
                DesiredSpeed = MaximumSlideSpeed;

                //calls the mommentum method
                Mommentum();
            }
        }
    }
}
