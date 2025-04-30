using UnityEngine;
using TMPro;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    // Prob Should be handled another way
    [SerializeField] private TextMeshProUGUI GameOverScore;

    private int score = 0;

    void Start()
    {
        UpdateScoreText();
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void ReducePoints(int points){
        if((score - points) < 0){
            score = 0;
        }else{
            score -= points;
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        GameOverScore.text = scoreText.text;    // Porb should handle somewhere else
    }   
}
