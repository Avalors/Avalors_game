using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    //boolean varible that confirms if player has reached the finish line
    public bool Finished = false;

    private void OnTriggerEnter(Collider other)
    {
        Finished = true;
    }
}
