using UnityEngine;
using System.Collections;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverGroup;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private float fadeInDuration = 1.5f;

    [SerializeField] GameObject gameManager;

    private bool isGameOver = false;

    void Start()
    {
        gameOverGroup.alpha = 0f;
        gameOverGroup.interactable = false;
        gameOverGroup.blocksRaycasts = false; 
   }

    public void ShowGameOverUI(bool winner){
        if(!isGameOver){
            gameOverText.text = winner ? "Winner!" : "Loser!";
            StartCoroutine(FadeIn());

            gameManager.GetComponent<CursorState>().ToggleCursorState();
        }
    }

    IEnumerator FadeIn(){

        isGameOver = true;
        float elapsed = 0f;
        while(elapsed < fadeInDuration){
            elapsed += Time.deltaTime;
            gameOverGroup.alpha = Mathf.Clamp01(elapsed/fadeInDuration);
            yield return null;
        }

        gameOverGroup.interactable = true;
        gameOverGroup.blocksRaycasts = true;
    }
}
