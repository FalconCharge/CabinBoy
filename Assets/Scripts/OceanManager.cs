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
        if (waveHeightMap == null)
        {
            Debug.LogWarning("waveHeightMap is not assigned!");
            return ocean.transform.position.y;
        }

        // Convert worldPos to local position relative to the ocean
        Vector3 localPos = ocean.transform.InverseTransformPoint(worldPos);

        // Normalize to UV coordinates (0-1 range)
        // 125 is the size of the mesh that should be normalized (It should be correct)
        float u = Mathf.Repeat(localPos.x / 125f + (Time.time * waveSpeed), 1f);
        float v = Mathf.Repeat(localPos.z / 125f + (Time.time * waveSpeed), 1f);

        Debug.Log(u.ToString() + ", " + v.ToString());

        // Sample the displacement map
        float heightSample = waveHeightMap.GetPixelBilinear(u, v).r * amplitude;

        Debug.Log(heightSample);
        // Reconstruct world Y using ocean base height and wave height scaling
        float height = ocean.transform.position.y + heightSample;

        return height;
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
