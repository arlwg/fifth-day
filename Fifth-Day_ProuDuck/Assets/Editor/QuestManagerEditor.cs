using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestManager))]
public class QuestManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        QuestManager questManager = (QuestManager) target;

        if (DrawDefaultInspector())
        {
            
        }
        if (GUILayout.Button("Complete Quests"))
        {
            questManager.CompleteAllQuests();
        }
        if (GUILayout.Button("Complete Quests Except One"))
        {
            questManager.CompleteAllQuestsButOne();
        }
    }
}