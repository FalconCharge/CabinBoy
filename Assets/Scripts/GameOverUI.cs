using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverGroup;
    [SerializeField] private float fadeInDuration = 1.5f;

    void Start()
    {
        gameOverGroup.alpha = 0f;
        gameOverGroup.interactable = false;
        gameOverGroup.blocksRaycasts = false;   
    }

    public void ShowGameOverUI(){
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn(){
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
