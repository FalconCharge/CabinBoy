using UnityEngine;

public class OceanManager : MonoBehaviour
{
    [SerializeField] private GameObject ocean;
    [SerializeField] private Texture2D waveHeightMap;
    [SerializeField] private float tiling = 1f; // match shader's "Normal Tiling" which is freq
    [SerializeField] private Vector2 panSpeed = new Vector2(0f, 0f); // from the shader
    [SerializeField] private float amplitude = 1f; // match shader amplitude
    [SerializeField] private float waveSpeed = 1f;


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

        v += Time.time * waveSpeed;

        // Clamp to prevent out-of-bounds access (if you're not using wrap mode)
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        // Sample the height from the red channel
        float heightSample = waveHeightMap.GetPixelBilinear(u, v).r * amplitude;

        return ocean.transform.position.y + heightSample;
    }






    void UpdateMaterials(){
        oceanMat.SetFloat("_Amptitude", amplitude);
        oceanMat.SetFloat("_PanSpeed", waveSpeed);
        oceanMat.SetFloat("_Frequency", tiling);
    }

    void OnDisable()
    {
        oceanMat.SetFloat("_PanSpeed", 0f);
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

}
