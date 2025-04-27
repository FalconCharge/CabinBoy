using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [Header("Wind Details")]
    [SerializeField] private float startGustTime = 10f;
    [SerializeField] private float timeToMaxGust  = 120f;
    [SerializeField] private Vector2 timeBTWGust = new Vector2(10f, 20f);
    [SerializeField] private Vector2 gustTimeAlive = new Vector2(3.0f, 8.0f);
    [Tooltip("How long leaves stay before gust takes place")]
    [SerializeField] private float bigGustDelay = 5.0f;

    [SerializeField] private float gustFactor = 0.0f;
    private WindManager windManager;

    private float nextGustIn   = 0f;
    private float currGustTime = 0f;
    private bool  isGusting    = false;


    [Header("Water Details")]
    [SerializeField] private float startWaterHeight = 1f;
    [SerializeField] private float maxWaterHeight = 15f;
    [SerializeField] private float startIncreasingHeight = 1.0f;
    [SerializeField] private float timeToMaxWaterHeight = 100f;
    [SerializeField] private float waterHeightFactor = 0.01f;


    private OceanManager oceanManager;


    // private vars
    private float totalTime = 0.0f;

    void Start()
    {
        // Init the Wind Manager
        windManager = GetComponent<WindManager>();

        // Init ocean manager
        oceanManager = GetComponent<OceanManager>();
        oceanManager.SetAmplitude(startWaterHeight);

        // TODO :
        // Init the spawn Manager
        // Spawn in Cargo on the ship

    }


    void Update()
    {
        totalTime += Time.deltaTime;

        // The Rise of the water
        WaterAmplitude();

        // The Winds affecting the ship
        Gust();
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
        windManager.StartGustWithDelay(Random.Range(-1.0f, 1.0f) * gustFactor, bigGustDelay);

        // Get gust length
        currGustTime = Random.Range(gustTimeAlive.x, gustTimeAlive.y);
    }

    private void ScheduleGust(){
        nextGustIn = Random.Range(timeBTWGust.x, timeBTWGust.y);
    }

    private void EndCurrentGust(){
        isGusting = false;
        windManager.StopWave();
        ScheduleGust();
    }

    // To be called within update and changes the Water height overtime
    private void WaterAmplitude() {
        if (startIncreasingHeight > totalTime) return;

        float elapsed = totalTime - startIncreasingHeight;
        waterHeightFactor = Mathf.Min(elapsed / timeToMaxWaterHeight, 1f);
        oceanManager.SetAmplitude(waterHeightFactor * maxWaterHeight);
    }

    // To be called within update and changes the gust power overtime
    private void Gust(){
        if(startGustTime > totalTime) return;

        float elapsed = totalTime - startGustTime;
        gustFactor = Mathf.Min(elapsed / timeToMaxGust, 1f);

        Gusting();
    }

    private void SpawnCargo(){
        //TODO: Spawn a bunch or cargo on the ship at the start of the journey
    }

}
