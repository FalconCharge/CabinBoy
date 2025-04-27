using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Wind Timing")]
    [Tooltip("No wind until totalTime >= windStartTime.")]
    [SerializeField] private float windStartTime = 10f;

    [Tooltip("Elapsed time at which windStrength == 1.")]
    [SerializeField] private float windFullTime  = 120f;

    [Tooltip("Min/Max seconds between gusts.")]
    [SerializeField] private Vector2 gustIntervalRange = new Vector2(10f, 20f);

    [Tooltip("Min/Max seconds that a single gust lasts.")]
    [SerializeField] private Vector2 gustTimeAlive = new Vector2(3.0f, 8.0f);

    [SerializeField] private float bigGustDelay = 15.0f;

    private WindManager windManager;

    private float totalTime    = 0f;
    private float nextGustIn   = 0f;
    private float currGustTime = 0f;
    private bool  isGusting    = false;

    private float windPower = 0.0f;

    void Start()
    {
        windManager = GetComponent<WindManager>();

    }


    void Update()
    {
        totalTime += Time.deltaTime;

        if(totalTime > windStartTime){
            Gusting();
        }

        if(windPower <= 1){
            windPower = windFullTime/totalTime;
        }
    }

    private void Gusting()
    {
        if (isGusting)
        {

            //Stop gusting when it's dead
            currGustTime -= Time.deltaTime;
            if (currGustTime <= 0f) EndCurrentGust();


        }
        else
        {
            nextGustIn -= Time.deltaTime;

            if(nextGustIn <= 0f){
                FireRandomGust();
            }
        }
    }

    // Starts a random gust either Port/Starboard 
    private void FireRandomGust(){
        isGusting = true;

        // Creates a gust with power with a relation to windPower and has a delay (particles spawn before the wind)                                        
        windManager.StartGustWithDelay(Random.Range(-1.0f, 1.0f) * windPower, bigGustDelay);

        // Get gust length
        currGustTime = Random.Range(gustTimeAlive.x, gustTimeAlive.y);
    }

    private void ScheduleGust(){
        nextGustIn = Random.Range(gustIntervalRange.x, gustIntervalRange.y);
    }

    private void EndCurrentGust(){
        isGusting = false;
        windManager.StopWave();
        ScheduleGust();
    }
}
