﻿#region

using UnityEditor;

#endregion

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Multiplicative))]
    public class MultiplicativeEditor : UnlitEditor
    {
        public override void OnInspectorGUI()
        {
            //Update
            serializedObject.Update();

            Type();
            Priority(40);
            Transparency();

            //Draw property groups
            if (propertyGroups != null)
                for (var i = 0; i < propertyGroups.Length; i++) propertyGroups[i].OnGUILayout();

            //Masking();
            ProjectionLimit();
            ForceForward();
            Instanced();

            //Delayed Mark
            ExecuteDelayedMark();
        }
    }
}