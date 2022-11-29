using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    //will store the position of the CameraPosition
    [SerializeField] Transform CameraPosition;

    
    void Update()
    {
        //sets the camera holder position to the Camera Position objects position
        transform.position = CameraPosition.position;
    }
}
