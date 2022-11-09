using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //allows us to add UI components from our unity game
using UnityEngine.SceneManagement;//allows us to add scenes from our unity game
using TMPro; //using Text mesh pro

public class MainMenuController : MonoBehaviour
{
    [Header("Volume setting")]
    [SerializeField] private TMP_Text volumeTextValue = null; //This initialise the volume number value on the right of the slider
    [SerializeField] private Slider volumeSlider = null; //This initialises the volume slider it self
    [SerializeField] private float defaultVolume = 0.5f; //This is the variable that holds the on-reset value of the volume

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null; //initialises our volume confirmation prompt

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

    public void SetVolume(float volume) // Method to chnage the audio
    {
        AudioListener.volume = volume; //sets volume to our float value
        volumeTextValue.text = volume.ToString("0.0"); //updates voulme text value when we change it
    }

    public void VolumeApply() //saves volume float to file called master volume 
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume); //stores applied volume locally
        StartCoroutine(ConfirmationBox()); //shows my prompt by calling upon the method like an embedded method
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio") //if the settings menu where used is the audio menu
        {
            AudioListener.volume = defaultVolume; //Sets volume to default
            volumeSlider.value = defaultVolume; //sets the value of the slider to default
            volumeTextValue.text = defaultVolume.ToString("0.0"); //setes value of the text value to the default
            VolumeApply(); //permenantly applies the default volume
        }
    }

    public IEnumerator ConfirmationBox() //shows prompt
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2); //shows prompt for 2 seconds
        confirmationPrompt.SetActive(false);
    }
}
