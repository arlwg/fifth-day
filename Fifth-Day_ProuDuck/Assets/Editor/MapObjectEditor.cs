using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapObject))]
public class MapObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapObject objectStage = (MapObject) target;
        if (DrawDefaultInspector())
        {
            
        }
        if (GUILayout.Button("Increase Object Stage"))
        {
            objectStage.UpdateStage();
        }
    }
}