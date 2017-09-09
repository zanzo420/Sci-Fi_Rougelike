﻿#region

using UnityEditor;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CollisionPrinter))]
    public class CollisionPrinterEditor : PrinterEditor
    {
        private SerializedProperty condition;
        private SerializedProperty conditionTime;
        private SerializedProperty layers;
        private SerializedProperty rotationSource;

        public override void OnEnable()
        {
            base.OnEnable();
            rotationSource = serializedObject.FindProperty("rotationSource");
            condition = serializedObject.FindProperty("condition");
            conditionTime = serializedObject.FindProperty("conditionTime");
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
            ConditionGUI();
            LayersGUI();
            RotationGUI();

            //Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        private void ConditionGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Condition", "When and how the decal is printed."));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(condition, new GUIContent("", "When and how the decal is printed."));
            if (condition.enumValueIndex == 1)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(conditionTime, new GUIContent("Delay", "How long after entry to delay printing the decal."));
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
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

        private void RotationGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Rotation Source", "What determines the rotation of our decal?"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(rotationSource, new GUIContent("", "What determines the rotation of our decal?"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}