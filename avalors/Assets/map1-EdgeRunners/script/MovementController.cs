using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    float PlayerHeight = 2f;

    //My basic movement variables 
    [Header("Movement")]
    //default movement speed  (will test and change hence is set it as a public variable)
    public float MovementSpeed = 6f;

    //My jumping variables
    [Header("Jumping")]
    //default jump force (will test and change)
    public float JumpForce = 5f;

    //my keybind variables
    [Header("Keybinds")]
    [SerializeField] KeyCode JumpKey = KeyCode.Space;

    //will speed up the movement speed
    public float MovementMultiplier = 10f;

    [Header("Drag")]
    //Ground and air drag values
    public float GroundDrag = 6f;
    public float AirDrag = 1f;

    //stores Horizontal and Vertical movement
    float HorizontalMovement;
    float VerticalMovement;

    //variable that checks if the player has hit the ground
    bool IsGrounded;

    //reference to my rigidbody character
    private Rigidbody rb;

    //stores the characters movement direction
    Vector3 MovementDirection; 

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
        CheckGround();
        Jump();
    }

    void MyInput()
    {
        // checks player input of w,a,s,d and return numerical value of 1 or -1 for x and y direction and saves to variable
        HorizontalMovement = Input.GetAxisRaw("Horizontal"); 
        VerticalMovement = Input.GetAxisRaw("Vertical"); 

        //establishes movement direction taking into consideration where the player is moving and the keys he/she presses
        MovementDirection = transform.forward * VerticalMovement + transform.right * HorizontalMovement; 
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

    void CheckGround()
    {
        //creates a ray to check if player is grounded, taking into account how the capsule is identified at its center
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight / 2 + 0.1f);
    }

    void Jump()
    {
        //selection statement checks if JumpKey is true (which is when space bar is pressed) and player is grounded
        if (Input.GetKeyDown(JumpKey) && IsGrounded)
        {
            //Adds a sudden force 
            rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
    }

    //has the same freqeuncy as the physics system
    private void FixedUpdate()
    { 
        MovementPlayer();
    }

    void MovementPlayer()
    {
        //causes player to accellerate in the correct direction
        rb.AddForce(MovementDirection.normalized * MovementSpeed * MovementMultiplier, ForceMode.Acceleration);
        //.normalized ensures the movement directions magnitude is equal on all sides, including the diagonals 
    }
}
