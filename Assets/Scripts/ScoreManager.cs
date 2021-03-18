using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    public int score;

    void Update()
    {
        DisplayScore();
        ShowHighScore();
    }

    void DisplayScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score.ToString();
        }

    }

    public void IncreaseScore(int amount)
    {
        score += amount;
    }


    #region PlayerPrefs Manager

    public void ShowHighScore()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore");
        }

    }


    #endregion
}
