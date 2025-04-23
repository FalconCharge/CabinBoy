using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class OceanManager : MonoBehaviour
{
    [SerializeField] private float ampitude = 0.5f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float waveSpeed = 1f;

    [SerializeField] private GameObject ocean;

    private Material oceanMat;

    void Start()
    {
        SetVariables();
    }

    void SetVariables(){
        oceanMat = ocean.GetComponent<Renderer>().sharedMaterial;
        UpdateMaterials();
    }

    void UpdateMaterials(){
        oceanMat.SetFloat("_Amptitude", ampitude/100);
        oceanMat.SetFloat("_Frequency", frequency/100);
        oceanMat.SetFloat("_PanSpeed", waveSpeed/100);
    }

    void OnDisable()
    {
        oceanMat.SetFloat("_PanSpeed", 0f);

    }
}
