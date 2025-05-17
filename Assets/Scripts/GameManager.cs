using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Spawning")]
    [SerializeField] private float spawnCratesTime = 5f;
    [SerializeField] private TextMeshProUGUI cargoText;
    [SerializeField] private TextMeshProUGUI timerText;
    


    private float textDisappearTime = 3f;
    private float textTimer = 0f;

    private bool isCargoSpawned = false;


    [Header("Timer")]
    [SerializeField] private float timerDuration = 100f;

    [SerializeField] CanvasGroup startUI;

    private OceanManager oceanManager;
    private CargoManager cargoManager;
    private GameTimer gameTimer;



    [Header("Island Movement")]
    [SerializeField] private GameObject island;
    [SerializeField] private Transform islandStartPoint;
    [SerializeField] private Transform islandEndPoint;
    [SerializeField] private float timerAdd = 8.0f;

    // private vars
    private float totalTime = 0.0f;

    private bool hasFadedout = false;

    void Start()
    {
        // Init the Wind Manager
        windManager = GetComponent<WindManager>();

        // Init ocean manager
        oceanManager = GetComponent<OceanManager>();
        oceanManager.SetAmplitude(startWaterHeight);

        cargoManager = GetComponent<CargoManager>();
        cargoText.alpha = 0f;

        gameTimer = GetComponent<GameTimer>();

        AudioManager.Instance.PlayMainTheme();

    }


    void Update()
    {
        totalTime += Time.deltaTime;

        // The Rise of the water
        WaterAmplitude();

        // The Winds affecting the ship
        Gust();

        // Handles Cargo Spawning
        Cargo();

        // Handles GameTimer
        HandleGameTimer();

        MoveIslandTowardsShip();
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
        float gustStrength;
        if(Random.Range(0f, 1f) < 0.5f){
            gustStrength = Random.Range(-1, -0.5f);
        }else{
            gustStrength = Random.Range(0.5f, 1f);
        }

                                             
        windManager.StartGustWithDelay(gustStrength * gustFactor, bigGustDelay);

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

    private void Cargo()
    {
        if (totalTime > spawnCratesTime && !isCargoSpawned)
        {
            cargoManager.SpawnCrates();
            isCargoSpawned = true;

            // Start running the timer
            gameTimer.StartTimer(timerDuration);

            // Show the cargo text and start fading it in
            StartCoroutine(FadeTextIn());
        }

        // Handle the text disappearing after a certain time
        if (isCargoSpawned && !hasFadedout)
        {
            textTimer += Time.deltaTime;

            if (textTimer >= textDisappearTime)
            {
                // Fade the text out
                StartCoroutine(FadeTextOut());
                hasFadedout = true;
            }
        }
    }

    private void HandleGameTimer(){
        if(gameTimer.IsDone()){
            if(cargoManager.HasCargo()){
                FindFirstObjectByType<GameOverUI>().ShowGameOverUI(true);
            }else{
                FindFirstObjectByType<GameOverUI>().ShowGameOverUI(false);
            }
            Debug.Log("GameOver");
        }
    }

    private void MoveIslandTowardsShip()
    {
        if (island == null || islandStartPoint == null || islandEndPoint == null)
            return;

        // float t = Mathf.Clamp01(totalTime / timerDuration);
        float t = timerDuration + timerAdd;
        float rawT   = Mathf.Clamp01(totalTime / t);
        float easedT = Mathf.SmoothStep(0f, 1f, rawT);
        island.transform.position = Vector3.Lerp(islandStartPoint.position, islandEndPoint.position, easedT);
    }


    // Coroutine to fade the text in
    private IEnumerator FadeTextIn()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1f;

        while (elapsedTime < fadeDuration)
        {
            cargoText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            timerText.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            startUI.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cargoText.alpha = 1f;
        timerText.alpha = 1f;
        startUI.alpha = 1f;
    }

    // Coroutine to fade the text out
    private IEnumerator FadeTextOut()
    {
        float elapsedTime = 0f;
        float fadeDuration = 1f;


        if(cargoText.alpha < 0.05f){
            cargoText.alpha = 0f;
            yield return null;
        }

        while (elapsedTime < fadeDuration)
        {
            cargoText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cargoText.alpha = 0f; 
        

    }
}
