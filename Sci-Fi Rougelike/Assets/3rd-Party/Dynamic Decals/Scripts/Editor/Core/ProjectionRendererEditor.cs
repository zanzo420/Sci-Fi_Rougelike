#region

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    [CustomEditor(typeof(ProjectionRenderer))]
    public class ProjectionRendererEditor : Editor
    {
        private bool delayedMark;
        private InstancedPropertyDrawer instancedDrawer;
        internal SerializedProperty maskMethod;
        internal SerializedProperty masks;

        internal SerializedProperty offset;

        //Renderer Properties
        internal SerializedProperty projection;

        //Property Drawers
        private ProjectionDrawer projectionDrawer;

        internal SerializedProperty properties;
        internal SerializedProperty tiling;

        public void OnEnable()
        {
            //Grab properties
            projection = serializedObject.FindProperty("projection");
            properties = serializedObject.FindProperty("properties");
            tiling = serializedObject.FindProperty("tiling");
            offset = serializedObject.FindProperty("offset");
            maskMethod = serializedObject.FindProperty("maskMethod");
            masks = serializedObject.FindProperty("masks");

            //Create our drawers
            projectionDrawer = new ProjectionDrawer(this, projection);
            instancedDrawer = new InstancedPropertyDrawer(this);

            //Register to undo redo
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public void OnDisable()
        {
            //Deregister from undo redo
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            //Require delayed update
            DelayedMark();

            //Repaint editor
            Repaint();
        }

        private void DelayedMark()
        {
            delayedMark = true;
        }

        public void ChangeProjection()
        {
            //Apply changes
            serializedObject.ApplyModifiedProperties();

            //Update projection
            foreach (var obj in serializedObject.targetObjects)
                ((ProjectionRenderer)obj).ChangeProjection();

            //Repaint the scene
            SceneView.RepaintAll();
        }

        public void UpdateProjection()
        {
            //Apply changes
            serializedObject.ApplyModifiedProperties();

            //Update projection
            foreach (var obj in serializedObject.targetObjects)
                ((ProjectionRenderer)obj).UpdateProjection();

            //Repaint the scene
            SceneView.RepaintAll();
        }

        public void ResetProperties()
        {
            //Record Undo
            foreach (var obj in serializedObject.targetObjects) Undo.RecordObject(obj, "Reset Properties");

            //Apply changes
            serializedObject.ApplyModifiedProperties();

            //Update editor and scene
            foreach (var obj in serializedObject.targetObjects)
                ((ProjectionRenderer)obj).ResetProperties(true);

            //Repaint the scene
            SceneView.RepaintAll();
        }

        public void MarkProperties()
        {
            //Apply changes
            serializedObject.ApplyModifiedProperties();

            //Update properties
            foreach (var obj in serializedObject.targetObjects)
                ((ProjectionRenderer)obj).MarkProperties(true);

            //Repaint the scene
            SceneView.RepaintAll();
        }

        public override void OnInspectorGUI()
        {
            //Update Object
            serializedObject.Update();

            EditorGUILayout.Space();

            //Draw drawers
            projectionDrawer.OnGUILayout();
            instancedDrawer.OnGUILayout();

            EditorGUILayout.Space();

            //Delayed Mark
            if (delayedMark)
            {
                //Update projection & properties
                foreach (var obj in serializedObject.targetObjects)
                    ((ProjectionRenderer)obj).ChangeProjection();

                //Repaint scene
                SceneView.RepaintAll();

                //No longer require undo
                delayedMark = false;
            }
        }
    }

    public class ProjectionDrawer
    {
        //Global properties
        private static float height = 400;

        private static ProjectionTab SelectedTab;

        private static bool foldout = true;
        private static Vector2 inspectorScrollPosition;
        private bool adjustingHeight;

        //Projection Inspector
        private Editor editor;

        private string newName = "New Projection";
        private readonly SerializedProperty projection;

        private int selectedOption;

        //Cached Variables
        private readonly ProjectionRendererEditor source;

        //Constructor
        public ProjectionDrawer(ProjectionRendererEditor Source, SerializedProperty Projection)
        {
            source = Source;
            projection = Projection;

            editor = Editor.CreateEditor(projection.objectReferenceValue);
        }

        //Primary Methods
        public void OnGUI(Rect position)
        {
            //Extend Rect to fill window
            var Rect = new Rect(position.x - 8, position.y, position.width + 6, position.height);

            //Height Adjustment
            if (adjustingHeight && Event.current.rawType == EventType.MouseUp && Event.current.button == 0)
            {
                adjustingHeight = false;
                Event.current.Use();
            }
            if (adjustingHeight)
            {
                //Resize
                if (Event.current.rawType == EventType.MouseDrag && Event.current.button == 0)
                {
                    height = Mathf.Clamp(Event.current.mousePosition.y - Rect.y, 140, 600);
                    Event.current.Use();
                }

                //Adjust Cursor
                EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.ResizeVertical);
            }

            if (projection.objectReferenceValue == null)
            {
                EditorGUI.DrawRect(Rect, LlockhamEditorUtility.MidgroundColor);
                GUI.BeginGroup(Rect);

                //Draw our Tabs
                DrawTab(new Rect(0, 0, Rect.width / 2, LlockhamEditorUtility.TabHeight), "New", ProjectionTab.New);
                DrawTab(new Rect(Rect.width / 2, 0, Rect.width / 2, LlockhamEditorUtility.TabHeight), "Existing", ProjectionTab.Existing);

                switch (SelectedTab)
                {
                    case ProjectionTab.New:
                        DrawNew(new Rect(0, LlockhamEditorUtility.TabHeight, Rect.width, Rect.height - LlockhamEditorUtility.TabHeight));
                        break;

                    case ProjectionTab.Existing:
                        DrawExisting(new Rect(0, LlockhamEditorUtility.TabHeight, Rect.width, Rect.height - LlockhamEditorUtility.TabHeight));
                        break;
                }
                GUI.EndGroup();
            }
            else
            {
                DrawInspector(Rect);
            }
        }

        public void OnGUILayout()
        {
            var layoutHeight = height;
            if (projection.objectReferenceValue == null)
                switch (SelectedTab)
                {
                    case ProjectionTab.New:
                        layoutHeight = 105;
                        break;
                    case ProjectionTab.Existing:
                        layoutHeight = 180;
                        break;
                }
            else if(!foldout) layoutHeight = 34;

            var rect = GUILayoutUtility.GetRect(0, layoutHeight);
            OnGUI(rect);
        }

        private static void DrawTab(Rect Rect, string Title, ProjectionTab Type)
        {
            var Selected = SelectedTab == Type;

            if (!Selected && Event.current.type == EventType.MouseDown && Event.current.button == 0 && Rect.Contains(Event.current.mousePosition))
            {
                SelectedTab = Type;
                Selected = true;

                Event.current.Use();
            }

            if (Selected) EditorGUI.DrawRect(Rect, LlockhamEditorUtility.MidgroundColor);
            else EditorGUI.DrawRect(Rect, LlockhamEditorUtility.BackgroundColor);

            EditorGUI.LabelField(Rect, Title, LlockhamEditorUtility.TabLabel);
        }

        #region Inspector

        private void DrawInspector(Rect Rect)
        {
            GUI.BeginGroup(Rect);

            //Object Area
            var ObjectArea = new Rect(4, 4, Rect.width - 8, 26);
            EditorGUI.DrawRect(ObjectArea, LlockhamEditorUtility.HeaderColor);

            //Field
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(30, 10, Rect.width - 60, 16), projection, new GUIContent(""));
            if (EditorGUI.EndChangeCheck()) source.ChangeProjection();

            //Foldout
            var Foldout = new Rect(12, 13, 10, 10);
            if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Foldout.Contains(Event.current.mousePosition))
            {
                foldout = !foldout;
                Event.current.Use();
            }
            GUI.DrawTexture(Foldout, foldout ? LlockhamEditorUtility.DownArrow : LlockhamEditorUtility.RightArrow);

            //Deselect
            var Close = new Rect(Rect.width - 24, 13, 8, 8);
            if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Close.Contains(Event.current.mousePosition))
            {
                if (EditorUtility.DisplayDialog("Clear Projection?", "Are you sure you want to change projection? All instanced variable changes will be lost.", "Continue", "Cancel"))
                {
                    //Set our new projection
                    projection.objectReferenceValue = null;

                    //Update properties
                    source.ChangeProjection();
                }
                Event.current.Use();
            }
            GUI.DrawTexture(Close, LlockhamEditorUtility.Cross);            

            if (foldout)
            {
                //Height Adjustment
                if (!adjustingHeight)
                {
                    var DragRect = new Rect(4, Rect.height - 4, Rect.width, 4);
                    EditorGUIUtility.AddCursorRect(DragRect, MouseCursor.ResizeVertical);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && DragRect.Contains(Event.current.mousePosition))
                    {
                        adjustingHeight = true;
                        Event.current.Use();
                    }
                }

                //Projection Editor
                var ScrollArea = new Rect(4, 30, Screen.width - 21, height - 34);
                EditorGUI.DrawRect(ScrollArea, LlockhamEditorUtility.BackgroundColor);

                GUILayout.BeginArea(new Rect(ScrollArea.x + 3, ScrollArea.y + 2, ScrollArea.width - 6, ScrollArea.height - 4));
                inspectorScrollPosition = GUILayout.BeginScrollView(inspectorScrollPosition, GUIStyle.none, GUIStyle.none);

                //Update our editor
                if (editor == null || editor.serializedObject.targetObject != projection.objectReferenceValue)
                    editor = Editor.CreateEditor(projection.objectReferenceValue);

                //Draw projection editor
                if (editor != null)
                {
                    EditorGUI.BeginChangeCheck();
                    editor.OnInspectorGUI();
                    if (EditorGUI.EndChangeCheck()) DynamicDecals.System.UpdateRenderers((Projection)projection.objectReferenceValue);
                }

                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            
            GUI.EndGroup();
        }

        #endregion

        #region New Tab

        private static void GenerateOptions()
        {
            //Get a list of all available projections
            var types = typeof(Projection).Assembly.GetTypes();
            optionValues = (from Type type in types where type.IsSubclassOf(typeof(Projection)) && !type.IsAbstract select type).ToArray();

            //Get a list of titles for all available projections
            options = new string[optionValues.Length];
            for (var i = 0; i < optionValues.Length; i++)
            {
                var type = optionValues[i];
                while (type != typeof(Projection))
                {
                    if (options[i] == null) options[i] = LlockhamEditorUtility.AddSpacesToType(type.Name);
                    else options[i] = LlockhamEditorUtility.AddSpacesToType(type.Name) + "/" + options[i];
                    type = type.BaseType;
                }
            }
        }

        private static string[] options;
        private static Type[] optionValues;

        private void DrawNew(Rect Rect)
        {
            //Generate Options
            GenerateOptions();

            //Backgroud
            GUI.BeginGroup(Rect);

            var FillArea = new Rect(4, 4, Rect.width - 8, Rect.height - 8);
            EditorGUI.DrawRect(FillArea, LlockhamEditorUtility.ForegroundColor);

            GUI.BeginGroup(FillArea);

            newName = EditorGUI.TextField(new Rect(5, 5, FillArea.width - 10, 18), "Name", newName);
            selectedOption = EditorGUI.Popup(new Rect(5, 25, FillArea.width - 10, 18), "Type", selectedOption, options);

            var CreateButton = new Rect((FillArea.width - 180) / 2, 50, 180, 18);
            EditorGUI.DrawRect(CreateButton, LlockhamEditorUtility.MidgroundColor);
            if (GUI.Button(CreateButton, "Create", EditorStyles.miniButton))
            {
                //Set our new projection
                projection.objectReferenceValue = CreateProjection(newName, optionValues[selectedOption]);
                
                //Update projection
                source.ChangeProjection();
            }

            GUI.EndGroup();
            GUI.EndGroup();
        }

        private static Projection CreateProjection(string Name, Type Type)
        {
            var projection = (Projection)ScriptableObject.CreateInstance(Type.Name);

            //Grab the path to our settings file
            var path = AssetDatabase.GetAssetPath(DynamicDecals.System.Settings);

            //Go up one directory, then into our projections folder
            path = path.TrimEnd('/');
            path = path.Remove(path.LastIndexOf('/') + 1);
            path = path.TrimEnd('/');
            path = path.Remove(path.LastIndexOf('/') + 1);
            path += "Projections/" + Name + ".asset";

            //Create our asset
            AssetDatabase.CreateAsset(projection, path);

            //Update our options
            FindAllProjections();

            return projection;
        }

        #endregion

        #region Existing Tab

        private static void GenerateTypeFilterOptions()
        {
            //Update our options
            GenerateOptions();

            typeFilterOptions = new string[options.Length + 1];

            typeFilterOptions[0] = "All";
            for (var i = 1; i < typeFilterOptions.Length; i++)
                typeFilterOptions[i] = options[i - 1];
        }

        private static string[] typeFilterOptions;

        private static void FindAllProjections()
        {
            //Find all projections
            var paths = AssetDatabase.FindAssets("t:Projection", null);
            existing = new Projection[paths.Length];

            //Load them all in
            for (var i = 0; i < paths.Length; i++)
                existing[i] = AssetDatabase.LoadAssetAtPath<Projection>(AssetDatabase.GUIDToAssetPath(paths[i]));
        }

        private static Projection[] existing;

        private static Vector2 scrollPosition;
        private static string searchQuery = "";
        private static int typeQuery;

        private void DrawExisting(Rect Rect)
        {
            //Generate Filter options
            GenerateTypeFilterOptions();

            //Find projections
            FindAllProjections();

            //Filters
            var FilterArea = new Rect(Rect.x + 4, Rect.y + Rect.height - 28, Rect.width - 8, 24);
            EditorGUI.DrawRect(FilterArea, LlockhamEditorUtility.ForegroundColor);

            GUI.BeginGroup(FilterArea);
            var searchbarWidth = FilterArea.width * 0.7f;

            searchQuery = EditorGUI.TextField(new Rect(4, 4, searchbarWidth, 20), searchQuery, EditorStyles.toolbarTextField);
            typeQuery = EditorGUI.Popup(new Rect(searchbarWidth + 8, 4, FilterArea.width - (searchbarWidth + 10), 20), typeQuery, typeFilterOptions);
            GUI.EndGroup();

            //Selections
            var SelectionArea = new Rect(Rect.x + 4, Rect.y + 4, Rect.width - 8, Rect.height - 32);
            EditorGUI.DrawRect(SelectionArea, LlockhamEditorUtility.BackgroundColor);
            GUI.BeginGroup(SelectionArea);

            var ScrollArea = new Rect(2, 2, SelectionArea.width - 4, SelectionArea.height - 4);
            scrollPosition = GUI.BeginScrollView(ScrollArea, scrollPosition, new Rect(0, 0, ScrollArea.width, existing.Length * 18 - 2), false, false, GUIStyle.none, GUIStyle.none);

            var j = 0;
            for (var i = 0; i < existing.Length; i++)
                if ((searchQuery == "" || existing[i].name.ContainsCaseInsenitive(searchQuery)) && (typeQuery == 0 || existing[i].GetType() == optionValues[typeQuery - 1]))
                {
                    if (DrawProjection(new Rect(0, j * 18, ScrollArea.width, 16), existing[i].name, existing[i].GetType().Name))
                    {
                        //Set our new projection
                        projection.objectReferenceValue = existing[i];

                        //Update projection
                        source.ChangeProjection();
                    }
                    j++;
                }

            GUI.EndScrollView();
            GUI.EndGroup();
        }

        private bool DrawProjection(Rect Rect, string Name, string Type)
        {
            //Draw Rect
            EditorGUI.DrawRect(Rect, LlockhamEditorUtility.ForegroundColor);

            //Draw Labels
            EditorGUI.LabelField(new Rect(Rect.x, Rect.y, Rect.width / 2, Rect.height), Name);
            EditorGUI.LabelField(new Rect(Rect.x + Rect.width / 2, Rect.y, Rect.width / 2, Rect.height), LlockhamEditorUtility.AddSpacesToType(Type));

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Rect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return true;
            }
            return false;
        }

        #endregion
    }
    internal enum ProjectionTab { New, Existing }

    public class InstancedPropertyDrawer
    {
        private readonly ProjectionRendererEditor source;

        public InstancedPropertyDrawer(ProjectionRendererEditor Source)
        {
            source = Source;
        }

        public void OnGUI(Rect position)
        {
            var projection = source.projection;
            var properties = source.properties;

            if (projection != null && projection.objectReferenceValue != null && properties != null && properties.isArray)
            {
                //Extend Rect to fill window
                var Area = new Rect(position.x - 8, position.y, position.width + 6, position.height);
                
                //Header
                EditorGUI.DrawRect(new Rect(Area.x + 4, Area.y, Area.width - 8, 24), LlockhamEditorUtility.HeaderColor);
                EditorGUI.LabelField(new Rect(Area.x + 8, Area.y + 4, Area.width - 24, 26), new GUIContent("Instanced Properties", "A projection shares values across all renderers that use it. Instanced properties override the projections values for this renderer only."), EditorStyles.boldLabel);

                //Reset
                var Reset = new Rect(Area.width - 20, Area.y + 6, 12, 12);
                if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Reset.Contains(Event.current.mousePosition))
                {
                    source.ResetProperties();
                    Event.current.Use();
                }
                GUI.DrawTexture(Reset, LlockhamEditorUtility.Reset);

                //Body
                EditorGUI.DrawRect(new Rect(Area.x + 4, Area.y + 24, Area.width - 8, Area.height - 24), LlockhamEditorUtility.MidgroundColor);
                var Group = new Rect(Area.x + 10, Area.y + 32, Area.width - 20, Area.height - 12);

                GUI.BeginGroup(Group);
                EditorGUI.BeginChangeCheck();

                //Generic Properties
                for (var i = 0; i < properties.arraySize; i++)
                {
                    //Grab properties
                    var name = properties.GetArrayElementAtIndex(i).FindPropertyRelative("name");
                    var type = properties.GetArrayElementAtIndex(i).FindPropertyRelative("type");
                    var enabled = properties.GetArrayElementAtIndex(i).FindPropertyRelative("enabled");

                    var color = properties.GetArrayElementAtIndex(i).FindPropertyRelative("color");
                    var value = properties.GetArrayElementAtIndex(i).FindPropertyRelative("value");

                    //Draw enabled field
                    EditorGUI.PropertyField(new Rect(Group.width - 15, i * 18 - 1, 15, 16), enabled, new GUIContent(""));

                    //Draw property field
                    switch (type.enumValueIndex)
                    {
                        case 0:
                            EditorGUI.PropertyField(new Rect(0, i * 18 - 1, Group.width - 20, 16), color, new GUIContent(name.stringValue));
                            break;
                        case 1:
                            EditorGUI.PropertyField(new Rect(0, i * 18 - 1, Group.width - 20, 16), value, new GUIContent(name.stringValue));
                            break;
                        case 2:
                            EditorGUI.PropertyField(new Rect(0, i * 18 - 1, Group.width - 80, 16), value, new GUIContent(name.stringValue));
                            EditorGUI.PropertyField(new Rect(Group.width - 80, i * 18 - 1, 60, 16), color, new GUIContent(""));
                            break;
                    }
                }

                //Tiling / Offset
                var tiling = source.tiling;
                var offset = source.offset;

                EditorGUI.LabelField(new Rect(0, properties.arraySize * 18 + 12, 80, 16), new GUIContent("Tiling"));
                EditorGUI.PropertyField(new Rect(80, properties.arraySize * 18 + 12, Group.width - 80, 16), tiling, new GUIContent(""));

                EditorGUI.LabelField(new Rect(0, properties.arraySize * 18 + 30, 80, 16), new GUIContent("Offset"));
                EditorGUI.PropertyField(new Rect(80, properties.arraySize * 18 + 30, Group.width - 80, 16), offset, new GUIContent(""));

                //Masking
                var maskMethod = source.maskMethod;
                var masks = source.masks;
                
                EditorGUI.LabelField(new Rect(0, properties.arraySize * 18 + 60, 60, 16), new GUIContent("Masking", "Determines which objects the projection can be drawn on."));
                EditorGUI.PropertyField(new Rect(80, properties.arraySize * 18 + 60, Group.width - 80, 16), maskMethod, new GUIContent(""));
                for (var i = 0; i < masks.arraySize; i++)
                {
                    var rect = new Rect(i == 0 || i == 2 ? 4 : Group.width / 2 + 4, properties.arraySize * 18 + (i < 2 ? 80 : 96), Group.width / 2 - 12, 16);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 8, rect.height), new GUIContent(DynamicDecals.System.Settings.Layers[i].name, ""), LlockhamEditorUtility.MiniLabel);
                    masks.GetArrayElementAtIndex(i).boolValue = EditorGUI.Toggle(new Rect(rect.xMax - 8, rect.y, 14, rect.height), new GUIContent(""), masks.GetArrayElementAtIndex(i).boolValue);
                }

                if (EditorGUI.EndChangeCheck())
                    source.MarkProperties();
                GUI.EndGroup();
            }
        }

        public void OnGUILayout()
        {
            var projection = source.projection;
            var properties = source.properties;

            if (projection != null && projection.objectReferenceValue != null && properties != null && properties.isArray)
            {
                var rect = EditorGUILayout.GetControlRect(true, properties.arraySize * 18 + 38 + 120);
                OnGUI(rect);
            }            
        }
    }
}