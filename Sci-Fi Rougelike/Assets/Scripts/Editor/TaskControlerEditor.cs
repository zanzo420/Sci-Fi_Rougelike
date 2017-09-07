//Copyright © Darwin Willers 2017

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TaskControler))]
public class UnitMoverEditor : Editor {
    
    private int _taskType;

    public override void OnInspectorGUI()
    {
        var script = (TaskControler) target;
        SerializedProperty taskType = _taskType;


        
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(_taskType)
        GUILayout.EndHorizontal();
        
      
    }


}