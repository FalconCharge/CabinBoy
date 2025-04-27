using UnityEngine;

public class OceanManager : MonoBehaviour
{
    [SerializeField] private GameObject ocean;
    [SerializeField] private Texture2D waveHeightMap;
    [SerializeField] private float tiling = 1f; // Please dont use I don't think it works
    [SerializeField] private Vector2 panSpeed = new Vector2(0f, 0f); 
    [SerializeField] private float amplitude = 1f; 

    private Material oceanMat;

    void Start()
    {
        SetVariables();
    }

    void SetVariables(){
        oceanMat = ocean.GetComponent<Renderer>().sharedMaterial;
        UpdateMaterials();
    }

    public float WaterHeightAtPosition(Vector3 worldPos)
    {

        // Map from world space (-125 to +125) to UV space (0 to 1)
        float u = (worldPos.x + 125f) / 250f;
        float v = (worldPos.z + 125f) / 250f;
        
        u += Time.time * panSpeed.x;
        v += Time.time * panSpeed.y;

        // Wrap UVs so they stay between 0 and 1
        u = Mathf.Repeat(u, 1f);
        v = Mathf.Repeat(v, 1f);

        // Sample the height from the red channel
        float heightSample = waveHeightMap.GetPixelBilinear(u, v).r * amplitude;

        return ocean.transform.position.y + heightSample;
    }




    void UpdateMaterials(){
        if(oceanMat != null){
            oceanMat.SetFloat("_Amptitude", amplitude);
            oceanMat.SetFloat("_Frequency", tiling);
            oceanMat.SetVector("_PanSpeed_1", panSpeed);
        }
    }

    void OnDisable()
    {
        oceanMat.SetVector("_PanSpeed_1", Vector2.zero);
    }

    void OnDrawGizmos() {
        float height = WaterHeightAtPosition(transform.position);
        Vector3 samplePoint = new Vector3(transform.position.x, height, transform.position.z);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(samplePoint, 0.2f);
    }
    // void OnDrawGizmos()
    // {
    //     Vector3 samplePos = transform.position;
    //     float h = WaterHeightAtPosition(samplePos);
    //     Gizmos.color = Color.cyan;
    //     Gizmos.DrawLine(
    //         samplePos + Vector3.up * (ocean.transform.position.y),     // base
    //         samplePos + Vector3.up * h                                  // sampled surface
    //     );
    // }


    void OnValidate()
    {
        UpdateMaterials();
    }

    public void SetAmplitude(float p_amplitude){
        if(p_amplitude >= 1){
            this.amplitude = p_amplitude;
        }else{
            this.amplitude = 1f;
        }
        UpdateMaterials();
    }

}
