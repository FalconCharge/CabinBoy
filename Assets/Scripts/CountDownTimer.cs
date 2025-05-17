using UnityEngine;

public class CountDownTimer
{

    private float currentTime ;
    private float startTime;
    private bool isFinished;

    public CountDownTimer(float duration)
    {
        startTime = Mathf.Max(duration, 0f);
        currentTime = startTime;
        isFinished = false;
    }
    public bool IsReady()
    {
        if (isFinished)
        {
            return true;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isFinished = true;
            return true;
        }

        return false;
    }

    public void Reset(){
        currentTime = startTime;
        isFinished = false;
    }

    public void SetDuration(float duration){
        startTime = Mathf.Max(duration, 0f);
        currentTime = startTime;
        isFinished = false;
    }
}
