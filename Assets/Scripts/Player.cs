using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Transform startPosition;

    [SerializeField] private GameObject gameOverUI;
    
    private PlayerScore playerScore;
    private CountDownTimer countDownTimer;

    private bool m_IsGameOver;

    void Start()
    {
        // A ref to the Score Text
        playerScore = GetComponent<PlayerScore>();
        // Sets up the Continous Score
        countDownTimer = new CountDownTimer(0.5f);

        // Moving to a starting Position
        if(startPosition != null){
            transform.position = startPosition.position;
        }

        m_IsGameOver = false;

        gameOverUI.SetActive(false);
    }

    void Update()
    {
        if(m_IsGameOver == false){
            ContinousPoints();
        }else{
            // Enable Game over UI
            gameOverUI.SetActive(true);
        }
    }


    // Updates the score by 1 each 0.5 seconds
    private void ContinousPoints()
    {
        if (countDownTimer.IsReady())
        {
            playerScore.AddPoints(1);
            countDownTimer.Reset();
        }
    }

    public void GameOver(){
        m_IsGameOver = true;
    }
}
