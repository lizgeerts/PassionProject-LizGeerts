using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{

    //team 1 = right
    //team 2 = left
    private int Team1GamePoints;
    private int Team2GamePoints;
    private int[] setPoints = { 0, 15, 30, 40, 0 };
    private int setPointsIndexTeam1 = 0;
    private int setPointsIndexTeam2 = 0;

    public BallLaunch ballLaunchScript;

    public GameObject team1SetText;
    public GameObject team2SetText;
    public GameObject team1GameText;
    public GameObject team2GameText;


    public void AddPoint()
    {
        if (ballLaunchScript.ballOnLeftSide)
        //if ball was missed on left side -> right side gets point = team1
        {
            setPointsIndexTeam1 += 1;
            UpdateGamePointsTeam1();
        }
        else if (!ballLaunchScript.ballOnLeftSide)
        {
            setPointsIndexTeam2 += 1;
            UpdateGamePointsTeam2();
        }
    }

    void UpdateGamePointsTeam1()
    {
        team1SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam1].ToString());
        if (setPointsIndexTeam1 == 4)
        {
            setPointsIndexTeam1 = 0;
            setPointsIndexTeam2 = 0;
            Team1GamePoints += 1;

            team1SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam1].ToString());
            team2SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam2].ToString());
            team1GameText.GetComponent<TextMeshProUGUI>().SetText(Team1GamePoints.ToString());
        }
    }

    void UpdateGamePointsTeam2()
    {
        team2SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam2].ToString());
        if (setPointsIndexTeam2 == 4)
        {
            setPointsIndexTeam1 = 0;
            setPointsIndexTeam2 = 0;
            Team2GamePoints += 1;

            team1SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam1].ToString());
            team2SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam2].ToString());
            team2GameText.GetComponent<TextMeshProUGUI>().SetText(Team2GamePoints.ToString());
        }
    }
}
