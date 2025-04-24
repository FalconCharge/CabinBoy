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

    public float WaterHeightAtPosition(Vector3 position)
    {
        // Vector3 localPos = position - ocean.transform.position;

        // // Apply tiling
        // float u = localPos.x * tiling;
        // float v = localPos.z * tiling;

        // // Apply pan/scrolling via time
        // u += Time.time * panSpeed.x * waveSpeed;
        // v += Time.time * panSpeed.y * waveSpeed;

        // // Wrap UVs to 0–1 range
        // u = u % 1f; if (u < 0f) u += 1f;
        // v = v % 1f; if (v < 0f) v += 1f;

        // float raw = waveHeightMap.GetPixelBilinear(u, v).r;
        // float height = (raw - 0.5f) * 4.25f * amplitude;

        // return ocean.transform.position.y + height;
        return 5;
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

    void OnValidate()
    {
        UpdateMaterials();
    }

}
