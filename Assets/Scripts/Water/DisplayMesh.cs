using System.Data.Common;
using UnityEngine;


[ExecuteAlways]
public class DisplayMesh : MonoBehaviour
{
    [Range(0, 250)]
    [SerializeField] private int size;
    
    [SerializeField] private float density;

    [SerializeField] private Material waterMeshMaterial;

    //[SerializeField] private float displacementAmplitudeTest;
    //[SerializeField] private float displacementScaleTest;

    public void GenerateMesh(){
        // Create thes Mesh
        MeshData meshData = MeshGenerator.GenerateWaterMesh(size, density);
        

        Mesh mesh = meshData.CreateMesh();

        // Ensure the GameObject has the required components
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        //ApplyStaticDisplacement(mesh);

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = waterMeshMaterial;

        /*
        if(Application.isPlaying){
            MeshDisplacer displayer = GetComponent<MeshDisplacer>();
            if(displayer == null) displayer = gameObject.AddComponent<MeshDisplacer>();
            displayer.Initialize(mesh);
        }
        */
    }

    void Start()
    {
        GenerateMesh();
    }


    // Only added to see in eidtor
    // private void ApplyStaticDisplacement(Mesh mesh)
    // {
    //     Vector3[] vertices = mesh.vertices;
    //     float offsetX = Random.Range(0, 100f);
    //     float offsetZ = Random.Range(0, 100f);

    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         Vector3 vertex = vertices[i];
    //         float x = vertex.x * displacementScaleTest + offsetX;
    //         float z = vertex.z * displacementScaleTest + offsetZ;
    //         vertices[i].y = Mathf.PerlinNoise(x, z) * displacementAmplitudeTest;
    //     }

    //     mesh.vertices = vertices;
    //     mesh.RecalculateNormals();
    // }





}
