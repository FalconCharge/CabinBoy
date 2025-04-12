using UnityEngine;

public class MeshDisplacer : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float scale = 0.2f;
    [SerializeField] private float scrollSpeed = 0.1f;

    private MeshFilter meshFilter;
    private Mesh originalMesh;
    private Vector3[] modifiedVertices;
    private float offsetX;
    private float offsetZ;

    public void Initialize(Mesh pOriginalMesh)
    {
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = pOriginalMesh;
        modifiedVertices = originalMesh.vertices;
        offsetX = Random.Range(0, 100f);
        offsetZ = Random.Range(0, 100f);
        
        ApplyDisplacement();
    }


    void Update()
    {
        if (originalMesh == null) return;
        
        offsetX += Time.deltaTime * scrollSpeed;
        offsetZ += Time.deltaTime * scrollSpeed;
        ApplyDisplacement();
    }

    private void ApplyDisplacement()
    {
        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 vertex = originalMesh.vertices[i];
            float x = vertex.x * scale + offsetX;
            float z = vertex.z * scale + offsetZ;
            modifiedVertices[i].y = Mathf.PerlinNoise(x, z) * amplitude;
        }
        
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Mesh mesh = meshFilter.sharedMesh;
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
    }


}