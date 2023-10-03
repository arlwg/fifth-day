using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CustomEditor(typeof(UpdateData), true)]
public class UpdateableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UpdateData data = (UpdateData) target;

        if (GUILayout.Button(("Update")))
        {
            data.NotifyOfUpdatedValues();
        }
    }
}
