using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class HUD : MonoBehaviour
{
    //list to store the players best times
    private List<float> FloatTimes;
    private List<string> StringTimes;

    //takes in external referenced scripts
    [Header("References")]
    [SerializeField] MovementController MC;
    [SerializeField] FinishLine FL;

    //sets text of Speed text to null
    [Header("UI components")]
    //My speed text
    [SerializeField] private TMP_Text SpeedText = null;
    //My timer text
    [SerializeField] private TMP_Text TimerText = null;
    //my final timer text
    [SerializeField] private TMP_Text FinalTimerObj = null;

    //will store the containers for the HUD and prerace container
    [SerializeField] private GameObject HUDContainer;
    [SerializeField] private GameObject PreRaceContainer;
    [SerializeField] private GameObject TimerObj;
    [SerializeField] private GameObject FinishLineContainer;
    [SerializeField] private GameObject NewPersonalBest;

    [Header("Keybinds")]
    [SerializeField] KeyCode Forward = KeyCode.W;

    [Header("Map to load")]
    //will store map selection menu
    public string _mapSelection;

    //sets timer to 0 on start
    float timer = 0;
    float RoundTimer = 0;

    //stores index value in float list
    private int Index;

    //boolean for timer function
    bool CanStartTimer = false;

    //boolean for ending the speed run
    bool IsFinished = false;
   
    private void FixedUpdate()
    {
        //will change the speed text at a frequency equivalent to the physics engine
        ChangeSpeedText();
    }

    //uses public speed from Movement controller and changes the text of speed text.
    void ChangeSpeedText()
    {
        SpeedText.text = "Speed= " + MC.RBSpeed;
    }

    private void Update()
    {
        OpenHud();
        Timer();
        FinishingConditions();
        FinishRun();
    }

    private void Start()
    {
        //loads data into Floattimes variable from player prefs
        FloatTimes = PlayerPrefsExtra.GetList("FloatTimes", new List<float>());

        //loads data into StringTimes variable from player prefs
        StringTimes = PlayerPrefsExtra.GetList("StringTimes", new List<string>());

        //sets it to false on start of the scene
        IsFinished = false;
    }


    void OpenHud()
    {
        //returns true when the player presses w
        if (Input.GetKeyDown(Forward))
        {
            CanStartTimer = true;

            //PreRaceContainer becomes invisible
            PreRaceContainer.SetActive(false);

            //HUD container becomes visible
            HUDContainer.SetActive(true);
        }
    }

    void Timer()
    {
        //runs the function while the boolean is true 
        if (CanStartTimer)
        {
            //sets timer to change in time 
            timer += Time.deltaTime;

            //calculates minutes and sets to string as an integer
            string Minutes = System.Math.Truncate(timer / 60f).ToString("00");

            //calulates the seconds and sets it to a string to 2d.p
            string Seconds = (timer % 60).ToString("00.00");

            //sets timer text
            TimerText.text = Minutes + ":" + Seconds;
        }
    }

    void FinishRun()
    {
        //if the player touches the finish line 
        if (IsFinished)
        {
            StopTimer();
            FinalHUD();
            StartCoroutine(ReturnToMSM());
            SortTimes();

            //sets leaderboard created to true, so that the leaderboard is constructed
            Leaderboard.LeaderboardCreated = true;
        }
    }

    void FinishingConditions()
    {
        //sets the boolean to true once the player reaches the finish line.
        if (FL.Finished && MapSelectionController.StartLevel)
        {
            IsFinished = true;

            //should make it iterate only once
            MapSelectionController.StartLevel = false;
        }
    }

    void StopTimer()
    {
        //sets the text in the finish line panel to the final time
        FinalTimerObj.text = TimerText.text;

        //type casts the deciaml data type to a float and rounds to 3d.p
        RoundTimer = (float)System.Math.Round(timer, 3);

        //Stops the timer
        CanStartTimer = false;
    }

    void FinalHUD()
    {
        //HUD Container turns invisible
        HUDContainer.SetActive(false);

        //turns timer in the corner off
        TimerObj.SetActive(false);

        //shows the finishLine container
        FinishLineContainer.SetActive(true);

    }

    void SortTimes()
    {
        //if the float times list is empty the time is automatically added to the strings list
        if (FloatTimes.Count == 0)
        {
            //adds the text item to the list
            StringTimes.Add(FinalTimerObj.text);

            //adds timer value to float list
            FloatTimes.Add(RoundTimer);
        }
        else
        {
            //adds timer value to float list
            FloatTimes.Add(RoundTimer);

            //sorts list using quicksort
            FloatTimes.Sort();

            //returns the index value of where the item is located after the sort
            Index = FloatTimes.IndexOf(RoundTimer);

            //shows personal best prompt if the timer is the smallest time
            if (Index == 0)
            {
                //shows new personal best response
                NewPersonalBest.SetActive(true);

                SortStringList();
            }
            //adds the item to the list if the list does not have 10 items yet
            else if (Index > (StringTimes.Count - 1) && Index < 9)
            {
                StringTimes.Add(FinalTimerObj.text);
            }
            else if (Index <= (StringTimes.Count - 1))
            {
                SortStringList();
            }
        }

        //adds the entire list into PlayerPrefs
        PlayerPrefsExtra.SetList("FloatTimes", FloatTimes);

        //adds the entire list into PlayerPrefs
        PlayerPrefsExtra.SetList("StringTimes", StringTimes);

        //sets it to false
        IsFinished = false;

    }

    private IEnumerator ReturnToMSM()
    {
        //suspends the execution of the rest of the co-routine till after 5 seconds is over
        yield return new WaitForSeconds(3);

        //sends player back to the map selection menu
        SceneManager.LoadScene(_mapSelection);

                //sets it to false once again once the player returns to map selection menu
        FL.Finished = false;

    }

    void SortStringList()
    {
        //temp variables
        string Temp = FinalTimerObj.text;
        string Temp2;

        //iterates the code for every value of the string to put it back in order
        for (int current = Index; current <= (9); current++)
        {
            //if the for loop has reached the item that is about to move list
            if (current > (StringTimes.Count - 1))
            {
                //adds it to the end of the list
                StringTimes.Add(Temp);

                //breaks the loop 
                break;
            }
            else
            {
                //replaces the value and stores the replaced value as a temporary variable.
                Temp2 = StringTimes[current];
                StringTimes[current] = Temp;
            }
            Temp = Temp2;
        }
    }
}
