using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{

    //team 1 = right
    //team 2 = left
    private int[] setPoints = { 0, 15, 30, 40, 0 };
    private int[] setPointIndex = new int[2];
    private int[] gamePoints = new int[2];

    public BallLaunch ballLaunchScript;

    public TextMeshProUGUI[] setTexts;
    public TextMeshProUGUI[] gameTexts;

    public GameManager gameManager;

    private bool hasNotTranstitioned = true;

    public void AddPoint()
    {
        if (ballLaunchScript.ballOnLeftSide)
        //if ball was missed on left side -> right side gets point = team1
        {
            AddSetPoint(0);
        }
        else if (!ballLaunchScript.ballOnLeftSide)
        {
            AddSetPoint(1);
        }
    }

    public void AddSetPoint(int team)
    {
        setPointIndex[team]++;

        // If they reached beyond 40, win game
        if (setPointIndex[team] == 4)
        {
            gamePoints[team]++;

            // Reset both teams set points
            setPointIndex[0] = 0;
            setPointIndex[1] = 0;

            UpdateAllSetTexts();
            UpdateGameText(team);

            return;
        }
        else
        {
            UpdateSetText(team);
        }
    }

    void UpdateSetText(int team)
    {
        setTexts[team].SetText(setPoints[setPointIndex[team]].ToString());
    }

    void UpdateGameText(int team)
    {
        Debug.Log("gamepoints:" + gamePoints[team]);

        gameTexts[team].SetText(gamePoints[team].ToString());

        if (!gameManager.transitionMultiverse
        && gamePoints[team] == gameManager.pointsTillChaos
        && hasNotTranstitioned)
        {
            gameManager.transitionMultiverse = true;
            hasNotTranstitioned = false;
        }
    }

    void UpdateAllSetTexts()
    {
        UpdateSetText(0);
        UpdateSetText(1);
    }
}
