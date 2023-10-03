using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovementQuest), true)]
public class QuestEditor : Editor
{
    
    SerializedProperty targetTagProp;

    private void OnEnable()
    {
        targetTagProp = serializedObject.FindProperty("targetTag");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(targetTagProp);
        targetTagProp.stringValue = EditorGUILayout.TagField("Target Tag", targetTagProp.stringValue);

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }
}