using UnityEngine;


/*
    Only use is to stop the shader from playing in the editor since it makes It too laggy

    Author: Ben Harper
*/
public class WaterShader : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private Vector2 direction = new Vector2(0, -1);

    private Material waterMaterial;

    void Start()
    {
        waterMaterial = GetComponent<MeshRenderer>().material;
        waterMaterial.SetFloat("_Speed", speed);
        waterMaterial.SetVector("_Direction", direction);

    }


}
