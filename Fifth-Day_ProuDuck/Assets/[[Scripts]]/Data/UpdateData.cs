using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
//Credit to Sebastian Lague - https://youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
public class UpdateData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate() {
#if UNITY_EDITOR
        if (autoUpdate) {
            EditorApplication.update += NotifyOfUpdatedValues;
        }
#endif
    }

    public void NotifyOfUpdatedValues() {
#if UNITY_EDITOR
        EditorApplication.update -= NotifyOfUpdatedValues;
#endif
        if (OnValuesUpdated != null) {
            OnValuesUpdated ();
        }
    }
}