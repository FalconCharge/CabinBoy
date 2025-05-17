using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] Slider progressionBar;
    [SerializeField] private float currTime = 1000f;

    private float maxTime;

    private bool isDone = false;

    void Update()
    {
        Timer();
    }

    private void Timer()
    {

        if (!isDone)
        {
            if (currTime <= 0)
            {
                currTime = 0f;
                isDone = true;
            }
            else
            {
                currTime -= Time.deltaTime;
            }
            UpdateUI();
        }
    }

    public void StartTimer(float duration)
    {
        isDone = false;
        maxTime = duration;
        currTime = duration;

        progressionBar.maxValue = duration;
        progressionBar.value = maxTime - currTime;


        UpdateUI();
    }

    public bool IsDone()
    {
        return isDone;
    }

    private void UpdateUI()
    {
        progressionBar.value = maxTime - currTime;
        timerText.text = "Timer: " + currTime.ToString("F2");
    }

    public float GetCurrentTime()
    {
        return currTime;
    }
}
