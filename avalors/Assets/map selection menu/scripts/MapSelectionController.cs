using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //will allow me to modify any UI components of my game 
using UnityEngine.SceneManagement; //will allow me to access my scenes

public class MapSelectionController : MonoBehaviour
{
    [Header("levels to load")] //appers as header in unity object interface
    public string _newGameLevel; //my level varialbe, i will write the name of my scene
    
    [Header("Main menu to load")]
    public string _mainMenuUI;

    //this is our boolean to check if we have started the level.
    public static bool StartLevel = false;

    public void edgeRunners_btn() //my load scene fuction, using void would allow it to be an on-click option
    {
        SceneManager.LoadScene(_newGameLevel); //will load my scene 
        StartLevel = true;
    }

    public void arrowBack_btn()
    {
        SceneManager.LoadScene(_mainMenuUI);
    }

}
