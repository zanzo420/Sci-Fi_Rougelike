#region

using UnityEditor;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(SceneLayers))]
    public class SceneLayersEditor : Editor
    {
        private SerializedProperty layers;

        private void OnEnable()
        {
            layers = serializedObject.FindProperty("layers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();

            var rect = GUILayoutUtility.GetRect(0, layers.arraySize * 20 + 40);
            MaskSettings(rect, layers);

            EditorGUILayout.Space();
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void MaskSettings(Rect Area, SerializedProperty Layers)
        {
            GUI.BeginGroup(Area);

            //Header
            EditorGUI.DrawRect(new Rect(0, 0, Area.width, 24), LlockhamEditorUtility.HeaderColor);
            EditorGUI.LabelField(new Rect(8, 4, Area.width - 32, 16), "Masking", EditorStyles.boldLabel);

            //Reset
            var Reset = new Rect(Area.width - 20, 6, 12, 12);
            if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Reset.Contains(Event.current.mousePosition))
            {
                ResetLayers(Layers);
                Event.current.Use();
            }
            GUI.DrawTexture(Reset, LlockhamEditorUtility.Reset);

            //Draw Background
            EditorGUI.DrawRect(new Rect(0, 24, Area.width, Area.height - 24), LlockhamEditorUtility.MidgroundColor);

            //Generate layer options
            for (var i = 0; i < Layers.arraySize; i++)
            {
                var layer = Layers.GetArrayElementAtIndex(i);
                var name = layer.FindPropertyRelative("name");
                var layers = layer.FindPropertyRelative("layers");

                var nameRect = new Rect(4, 32 + i * 20, Area.width - 160, 16);
                var layerRect = new Rect(Area.width - 140, 32 + i * 20, 120, 16);

                EditorGUI.PropertyField(nameRect, name, GUIContent.none);
                EditorGUI.PropertyField(layerRect, layers, GUIContent.none);
            }

            GUI.EndGroup();
        }

        private void ResetLayers(SerializedProperty Layers)
        {
            Layers.arraySize = 4;
            for (var i = 0; i < Layers.arraySize; i++)
            {
                var layer = Layers.GetArrayElementAtIndex(i);
                var name = layer.FindPropertyRelative("name");
                var layers = layer.FindPropertyRelative("layers");

                name.stringValue = "Layer " + (i + 1);
                layers.intValue = 0;
            }

            Layers.serializedObject.ApplyModifiedProperties();
        }
    }
}