using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //allows us to add UI components from our unity game
using UnityEngine.SceneManagement; //allows us to add scenes from our unity game

public class MainMenuController : MonoBehaviour
{
    [Header("map selection to load")]
    public string _mapSelection; //what i intend to run as the map selection scene

    public void startgamedialogue_yesbutton()//allow us to control the on click event
    {
        SceneManager.LoadScene(_mapSelection); //this allows us to load a scene that we attach, when we click the yes button
    }

    public void exitgamedialogue_yesbutton() //closes the game when we press the yes button on the exit game dialogue button
    {
        Application.Quit();
    }
}
