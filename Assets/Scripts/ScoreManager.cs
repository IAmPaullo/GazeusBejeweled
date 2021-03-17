using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    public TextMeshProUGUI scoreText;
    public int score;

    void Update()
    {
        scoreText.text = "" + score.ToString();
    }


    public void IncreaseScore(int amount)
    {
        score += amount;
    }
}
