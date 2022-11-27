using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //sensativity variables: set to serialize field to allow us to edit the private variables (attributes) in the inspector
    [SerializeField] private float SensitivityX = 360f; 
    [SerializeField] private float SensitivityY = 360f;

    //our player camera
    Camera cam; 

    //horizontal and vertical mouse movement varialbles 
    float MouseX;
    float MouseY; 

    //stores multiplier
    float Multiplier = 0.01f;

    //float variables for x and y rotation angles
    float xRotation;
    float yRotation;

    private void Start()
    {
        //stores camera object to 'cam' variable
        cam = GetComponentInChildren<Camera>();

        CursorState();
    }

    void CursorState()
    {
        //makes cursor invislbe and locks it to the center of the screen.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MyInput();
        LookAround();
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
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //to set player rotation
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
