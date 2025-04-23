using UnityEngine;


/*
    Only use is to stop the shader from playing in the editor since it makes It too laggy

    Author: Ben Harper
*/
public class WaterShader : MonoBehaviour
{
    [SerializeField] private float panSpeed = 0.1f;

    private Material waterMaterial;

    void Start()
    {
        waterMaterial = GetComponent<MeshRenderer>().material;
        waterMaterial.SetFloat("_Pan_Speed", panSpeed);
    }


}
