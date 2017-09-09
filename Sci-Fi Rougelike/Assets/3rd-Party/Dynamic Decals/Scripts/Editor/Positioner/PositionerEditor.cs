#region

using UnityEditor;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Positioner))]
    public abstract class PositionerEditor : Editor
    {
        private SerializedProperty alwaysVisible;
        private SerializedProperty layers;
        private SerializedProperty projection;

        public virtual void OnEnable()
        {
            projection = serializedObject.FindProperty("projection");
            layers = serializedObject.FindProperty("layers");
            alwaysVisible = serializedObject.FindProperty("alwaysVisible");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            projectionGUI();
            CastGUI();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        protected void projectionGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Decal", "The Decal to project"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(projection, new GUIContent("", "The Decal to project"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        protected void CastGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Cast", "Details about how we cast the decal into the scene"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(layers, new GUIContent("Layers", "The Layers to cast onto"));
            EditorGUILayout.PropertyField(alwaysVisible, new GUIContent("Always Visible", "Should we hide the projection when our cast hits nothing?"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}