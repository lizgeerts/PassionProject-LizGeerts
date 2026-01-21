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
    private int[] setPoints = { 0, 15, 30, 40 };
    private int setPointsIndexTeam1 = 0;
    private int setPointsIndexTeam2 = 0;

    public Ballcontroller ballScript;

    public GameObject team1SetText;
    public GameObject team2SetText;
    public GameObject team1GameText;
    public GameObject team2GameText;

    public bool pointAdded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //team1GameText.GetComponent<TextMeshProUGUI>().SetText("1");

    }

    // Update is called once per frame
    void Update()
    {
        if (pointAdded == true)
        {
            return;
        }

        if (ballScript.bounceCount == 2 && ballScript.leftSide)
        //if ball bounced twice on left side -> right side gets point = team1
        {
            setPointsIndexTeam1 += 1;
            UpdateGamePointsTeam1();
            pointAdded = true;
        }
        else if (ballScript.bounceCount == 2 && ballScript.rightSide)
        {
            setPointsIndexTeam2 += 1;
            UpdateGamePointsTeam2();
            pointAdded = true;
        }
    }

    void UpdateGamePointsTeam1()
    {
        team1SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam1].ToString());
        if (setPointsIndexTeam1 == 3)
        {
            setPointsIndexTeam1 = 0;
            setPointsIndexTeam2 = 0;
            Team1GamePoints += 1;
            team1GameText.GetComponent<TextMeshProUGUI>().SetText(Team1GamePoints.ToString());
        }
    }

    void UpdateGamePointsTeam2()
    {
        team2SetText.GetComponent<TextMeshProUGUI>().SetText(setPoints[setPointsIndexTeam2].ToString());
        if (setPointsIndexTeam2 == 3)
        {
            setPointsIndexTeam1 = 0;
            setPointsIndexTeam2 = 0;
            Team2GamePoints += 1;
            team2GameText.GetComponent<TextMeshProUGUI>().SetText(Team2GamePoints.ToString());
        }
    }
}
