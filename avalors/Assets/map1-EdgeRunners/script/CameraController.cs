using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    //attribute to store wall running script 
    [SerializeField] WallRunning WallRun;
    [SerializeField] FinishLine FinishLine;

    //sensativity variables: set to serialize field to allow us to edit the private variables (attributes) in the inspector
    [SerializeField] private float SensitivityX = 360f; 
    [SerializeField] private float SensitivityY = 360f;

    //To reference our camera and orientation
    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    //horizontal and vertical mouse movement varialbles 
    float MouseX;
    float MouseY; 

    //stores multiplier
    float Multiplier = 0.01f;

    //float variables for x and y rotation angles
    float xRotation;
    float yRotation;

    bool IsFinished = false;

    private void Start()
    {
        IsFinished = false;
    }

    void CursorState()
    {
        //under the condition that the player has not reached the finish line
        if (!IsFinished)
        {
            //makes cursor invislbe and locks it to the center of the screen.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //if the boolearn returns true
        else if (IsFinished)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FinishingConditions()
    {
        if (FinishLine.Finished)
        {
            IsFinished = true;
        }
    }

    private void Update()
    {
        MyInput();
        LookAround();
        CursorState();
        FinishingConditions();
    }

    void MyInput()
    {
        //sets variable to the respective input axis
        MouseX = Input.GetAxisRaw("Mouse X");
        MouseY = Input.GetAxisRaw("Mouse Y");

        //rotation on the y axis is comparable to horizontal rotation
        yRotation += MouseX * SensitivityX * Multiplier;

        //rotation on the x axis makes our player look up and down, if we added the up and down would be inverted
        xRotation -= MouseY* SensitivityY * Multiplier;

        //Here I clamped our x rotation so the player cannot look tofar upward or downwards
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
    
    void LookAround()
    {
        //to set camera rotation
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, WallRun.Tilt);
        //to set player rotation
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
