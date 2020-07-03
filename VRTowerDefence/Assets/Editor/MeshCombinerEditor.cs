using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombinerScript))]
public class MeshCombinerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshCombinerScript myScript = (MeshCombinerScript)target;

        if (GUILayout.Button("BuildMyMesh"))
        {
            myScript.MyOwnAdvancedMeshCombinder();
        }

        if (GUILayout.Button("DestroyMesh"))
        {
            myScript.DestroyMesh();
        }
    }
}
