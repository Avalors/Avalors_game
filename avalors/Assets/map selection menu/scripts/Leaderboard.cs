using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    //created a private list attribute to sotre the players best times
    private List<string> BestTimesList;

    [Header("UI components")]
    [SerializeField] private TMP_Text BestTimesText;

    //leaderboard boolean that will be turned to true once again once the player reaches the finish line
    public static bool LeaderboardCreated = true;

    //stores the top leaderboard lines
    private string TopTen;

    // Update is called once per frame
    void Update()
    {
        TimedRanking();
    }

    void TimedRanking()
    {
        if (!MapSelectionController.StartLevel && LeaderboardCreated)
        {
            //loads data into StringTimes variable from player prefs
            BestTimesList = PlayerPrefsExtra.GetList("StringTimes", new List<string>());

            //replaces each line in the text with leaderboard scores
            for (int lines = 0; lines <= BestTimesList.Count-1; lines++)
            {
                //stores the position in top 10 times of item
                int Ranker = lines + 1;
                
                //adds the item to the text mesh pro object
                TopTen += Ranker + ".  " + BestTimesList[lines] + System.Environment.NewLine;
            }
            //stores the top ten string into the text
            BestTimesText.text = TopTen;

            //sets the function if statement parameter to false so it only iterates once
            LeaderboardCreated = false;

            //resets the string back to empty
            TopTen = "";
        }
    }
}
