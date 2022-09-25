using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScreen : MonoBehaviour
{
    public int score;
    public int highScore;

    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] GameObject Congratulation;

    private void OnEnable()
    {
        Congratulation.SetActive(false);
        ScoreText.text = score.ToString("Your Score: 00");
        HighScoreText.text = highScore.ToString("High Score: 00");

        if (score > highScore)
        {
            Congratulation.SetActive(true);
            HighScoreText.text = score.ToString("High Score: 00");
        }
    }
}
