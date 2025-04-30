using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(DisplayMesh))]
public class WaterGeneratorEditor : Editor
{
    // Operates similar to Update
    public override void OnInspectorGUI()
    {
        // Connects the public values to change
        DrawDefaultInspector();

        DisplayMesh meshGenerator = (DisplayMesh)target;

        // Creates a Button you can press
        if(GUILayout.Button("Generate")){
            meshGenerator.GenerateMesh();
        }
    }
}
