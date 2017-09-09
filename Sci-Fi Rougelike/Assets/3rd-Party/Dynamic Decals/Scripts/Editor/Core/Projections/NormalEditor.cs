using UnityEngine;
using UnityEditor;
using System.Collections;

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(Normal))]
    public class NormalEditor : ProjectionEditor
    {
        //Property groups
        protected SerializedProperty normal;

        //Property group drawers
        protected NormalTextureDrawer normalDrawer;

        public override void OnEnable()
        {
            base.OnEnable();

            if (target != null)
            {
                //Grab our properties
                normal = serializedObject.FindProperty("normal");

                //Initialize our property drawers
                if (propertyGroups == null || propertyGroups.Length != 1) propertyGroups = new PropertyGroupDrawer[1];
                if (propertyGroups[0] == null) propertyGroups[0] = new NormalTextureDrawer(new GUIContent("Normal"), normal, this);

                //Initialize property groups
                if (propertyGroups != null) for (int i = 0; i < propertyGroups.Length; i++) propertyGroups[i].Initialize();
            }
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            //Update
            serializedObject.Update();

            Type();
            Priority(40);
            Transparency(false);

            //Draw property groups
            if (propertyGroups != null)
            {
                for (int i = 0; i < propertyGroups.Length; i++) propertyGroups[i].OnGUILayout();
            }

            //Masking();
            ProjectionLimit();
            ForceForward();
            Instanced();

            //Delayed Mark
            ExecuteDelayedMark();
        }
    }
}