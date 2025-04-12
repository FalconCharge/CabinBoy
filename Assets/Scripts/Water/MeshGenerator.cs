using UnityEngine;

public static class MeshGenerator
{

    public static MeshData GenerateWaterMesh(int size, float density){

        int verticesPerLine = Mathf.Max(2, Mathf.RoundToInt(size * density) + 1);
        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        float unitSize = 1f / density;

        float halfSize = size/2f;

        for(int x = 0; x < verticesPerLine; x++){
            for(int y = 0; y < verticesPerLine; y++){


                meshData.vertices[vertexIndex] = new Vector3(unitSize * x - halfSize, 0, unitSize * y - halfSize);
                meshData.uvs[vertexIndex] = new Vector2(x/(float) (verticesPerLine -1), y/(float)(verticesPerLine -1));

                if(x < verticesPerLine -1 && y < verticesPerLine -1){

                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);

                }

                vertexIndex++;
            }
        }

        

        return meshData;
    }

}
