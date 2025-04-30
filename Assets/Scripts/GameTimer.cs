using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float currTime = 1000f;

    private bool isDone = false;

    void Update()
    {
        Timer();
    }

    private void Timer(){

        if(!isDone){
            if(currTime <= 0){
                currTime = 0f;
                isDone = true;
            }else{
                currTime -= Time.deltaTime;
            }
            UpdateText();
        }
    }

    public void StartTimer(float duration){
        isDone = false;
        currTime = duration;
    }

    public bool IsDone(){
        return isDone;
    }

    private void UpdateText(){
        timerText.text = "Timer: " + currTime.ToString("F2");
    }
}
