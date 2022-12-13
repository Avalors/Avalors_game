using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WayPointMarker : MonoBehaviour
{
    //will store my image
    public Image WayPoint;

    //My finish line object 
    public Transform FinishLine;

    //my player camera
    [SerializeField] Camera cam;

    //distance text
    [SerializeField] private TMP_Text DistanceText;

    //offset vector
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        SettingPosition();
        SettingText();
    }

    void SettingPosition()
    {
        //will ensure that the icon does not go off the players screen
        float MinX = WayPoint.GetPixelAdjustedRect().width / 2;
        float MaxX = Screen.width - MinX;

        float MinY = WayPoint.GetPixelAdjustedRect().height / 2;
        float MaxY = Screen.height - MinY;

        // converts 2d point to a 3d point on the finish line object
        Vector2 pos = cam.WorldToScreenPoint(FinishLine.position + offset);

        //fixes the position of the waypiont so that it does not go off the screen.
        pos.x = Mathf.Clamp(pos.x, MinX, MaxX);
        pos.y = Mathf.Clamp(pos.y, MinY, MaxY);

        //will return true if the finish line is behind of the player, and false if it is ahead
        if (Vector3.Dot((FinishLine.position - transform.position), cam.transform.forward) < 0)
        {
            if (pos.x < Screen.width / 2)
            {
                //if the way point appears on the left side it is on the right and vice versa
                pos.x = MaxX;
            }
            else
            {
                pos.x = MinX;
            }
        }

        //sets the image position
        WayPoint.transform.position = pos;
    }

    void SettingText()
    {
        //calculates the distance between the finish line object and the player position, then typecasts the distance as an integer then into a string
        //and saves this into the text for the way point marker
        DistanceText.text = ((int)Vector3.Distance(FinishLine.position, transform.position)).ToString() + "m";
    }
}
