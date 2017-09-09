#region

using UnityEditor;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RayPrinter))]
    public class RayPrinterEditor : PrinterEditor
    {
        private SerializedProperty layers;

        public override void OnEnable()
        {
            base.OnEnable();

            layers = serializedObject.FindProperty("layers");
        }

        public override void OnInspectorGUI()
        {
            //Update object
            serializedObject.Update();

            PrintGUI();
            BehaviourGUI();
            PoolGUI();
            ParentGUI();
            OverlapGUI();
            FrequencyGUI();
            LayersGUI();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void LayersGUI()
        {
            if (prints.arraySize > 1 && printMethod.enumValueIndex == 2)
            {
                var finalLayers = 0;
                foreach (SerializedProperty layermask in printLayers)
                    finalLayers = finalLayers | layermask.intValue;
                layers.intValue = finalLayers;
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("Layers", "Which layers to cast against"));
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(layers, new GUIContent("", "Which layers to cast against"));
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
    }
}