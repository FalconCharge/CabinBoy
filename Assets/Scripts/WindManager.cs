using UnityEngine;

public class WindManager : MonoBehaviour
{
    [Header("Speed of Force Change")]
    [SerializeField] private float windChangeSpeed = 1f;

    [Header("Max Tilt Amount (in units)")]
    [Range(0, 2.0f)]
    [SerializeField] private float maxTilt = 0.5f;

    [Header("Buoyancy Points")]
    [SerializeField] private Transform portBPoint;
    [SerializeField] private Transform starboardBPoint;

    [Header("Particles")]
    [SerializeField] private ParticleSystem portParticle;
    [SerializeField] private ParticleSystem starBoardParticle;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float particlesAtStrength = 0.5f;

    private Vector3 portOrig;
    private Vector3 starboardOrig;

    // runtime wind strength: negative = port side, positive = starboard
    private float currentWind = 0f;
    private float targetWind = 0f;


    private float timeToNextGust = 0f;

    private bool hasWave = false;

    void Start()
    {
        portOrig = portBPoint.localPosition;
        starboardOrig = starboardBPoint.localPosition;

        StopPortParticles();
        StopStarboardParticle();
    }

    void Update()
    {

        timeToNextGust -= Time.deltaTime;

        if(timeToNextGust < 0){
            UpdatePointHeight();
        }

        if(targetWind != 0){
            hasWave = true;
        }else{
            hasWave = false;
        }

        if (targetWind != 0)
        {
            // Determine side and absolute strength
            string side     = targetWind > 0f ? "Starboard" : "Port";
            float strength  = Mathf.Abs(targetWind);

            int val = Mathf.RoundToInt(strength * 10f);
            Debug.Log($"Wind -> {side} @ {val}/10");
        }
        else
        {
            Debug.Log("Wind → None");
        }
        
    }

    private void UpdatePointHeight()
    {
        // 1) currentWind -> targetWind
        currentWind = Mathf.MoveTowards(currentWind, targetWind, windChangeSpeed * Time.deltaTime);

        // 2) figure out how much each side drops
        float portDrop = Mathf.Max(0f, -currentWind) * maxTilt;
        float starboardDrop = Mathf.Max(0f, currentWind) * maxTilt;

        // 3) set targets
        Vector3 portTarget = portOrig + Vector3.down * portDrop;
        Vector3 starboardTarget = starboardOrig + Vector3.down * starboardDrop;

        // 4) lerp to points
        portBPoint.localPosition = Vector3.Lerp(portBPoint.localPosition, portTarget, windChangeSpeed * Time.deltaTime);
        starboardBPoint.localPosition = Vector3.Lerp(starboardBPoint.localPosition, starboardTarget, windChangeSpeed * Time.deltaTime);
    }



    public void StartGustWithDelay(float strength, float delay){
        // Ignore If currently Making a Dust
        if(hasWave) return;

        timeToNextGust = delay;
        targetWind = Mathf.Clamp(strength, -1, 1);

        if(Mathf.Abs(targetWind) < particlesAtStrength){     // WARNING MAGIC NUMBER!!
            return;
        }else if(targetWind > 0){
            StartStarboardParticle();
        }else{
            StartPortParticle();
        }
        
        
    }

    public bool HasWave(){
        return hasWave;
    }

    public void StopWave()
    {
        if(targetWind < particlesAtStrength){
            targetWind = 0.0f;
            return;
        }

        if(targetWind > 0){
            StopStarboardParticle();
        }else{
            StopPortParticles();
        }
        
        targetWind = 0f;
    }

    private void StartPortParticle(){
        if(portParticle.isPlaying){
            Debug.LogWarning("Port particles was already playing");
        }else{
            portParticle.Play();
        }
    }
    private void StopPortParticles(){
        if(!portParticle.isPlaying){
            Debug.LogWarning("Port particles was already stopped");
        }else{
            portParticle.Stop();
        }
    }

    private void StartStarboardParticle(){
        if(starBoardParticle.isPlaying){
            Debug.LogWarning("starboard particles was already playing");
        }else{
            starBoardParticle.Play();
        }
    }
    private void StopStarboardParticle(){
        if(!starBoardParticle.isPlaying){
            Debug.LogWarning("starboard particles was already stopped");
        }else{
            starBoardParticle.Stop();
        }
    }

}
