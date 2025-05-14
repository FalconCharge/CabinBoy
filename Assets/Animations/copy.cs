// Save as: Assets/Editor/CopyAllComponents.cs
using UnityEditor;
using UnityEngine;

public class copy : EditorWindow
{
    [MenuItem("Tools/Copy All Components %#&c")] // Ctrl/Cmd+Shift+C
    static void CopySelected()
    {
        var sel = Selection.gameObjects;
        if (sel.Length != 2)
        {
            Debug.LogWarning("Select exactly TWO GameObjects: source then destination.");
            return;
        }

        var source = sel[0];
        var target = sel[1];

        foreach (var srcComp in source.GetComponents<Component>())
        {
            // Skip Transform (always present)
            if (srcComp is Transform) continue;

            // Copy component data  
            UnityEditorInternal.ComponentUtility.CopyComponent(srcComp);
            // Paste as a new component on the target  
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
        }

        Debug.Log($"Copied {source.GetComponents<Component>().Length - 1} components from '{source.name}' to '{target.name}'.");
    }

    // You can also add a validator so the menu is only enabled when exactly two objects are selected:
    [MenuItem("Tools/Copy All Components %#c", true)]
    static bool CopySelected_Validate()
    {
        return Selection.gameObjects.Length == 2;
    }
}
