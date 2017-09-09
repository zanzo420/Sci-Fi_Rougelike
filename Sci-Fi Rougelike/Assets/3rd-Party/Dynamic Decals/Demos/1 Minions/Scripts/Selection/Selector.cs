#region

using System.Collections.Generic;
using LlockhamIndustries.Decals;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(GenericCameraController))]
    public class Selector : MonoBehaviour
    {
        private static Selector selector;

        private List<Selectable> available;
        public ProjectionRenderer border;
        public LayerMask boxSelect;

        private BoxSelection boxSelection;

        //Backing fields
        private GenericCameraController controller;

        [Header("Projections")]
        public ProjectionRenderer innards;

        [Header("Layers")]
        public LayerMask select;

        private List<Selectable> softSelection;
        public float width = 0.05f;

        //Singleton
        public static bool Initialized
        {
            get { return selector != null; }
        }

        //Selection
        public List<Selectable> Selection { get; private set; }

        private void Awake()
        {
            if (selector == null) selector = this;
            else if (selector != this) Destroy(this);
        }

        //Register - deregister
        public static void Register(Selectable Selectable)
        {
            //Initialize
            if (selector.available == null) selector.available = new List<Selectable>();

            //Register new selectable
            if (!selector.available.Contains(Selectable)) selector.available.Add(Selectable);
        }

        public static void Deregister(Selectable Selectable)
        {
            //Deregister selectable
            if (selector.available != null) selector.available.Remove(Selectable);
            if (selector.Selection != null) selector.Selection.Remove(Selectable);
            if (selector.softSelection != null) selector.softSelection.Remove(Selectable);
        }

        //Generic methods
        private void Start()
        {
            controller = GetComponent<FreeCameraController>();
        }

        private void Update()
        {
            //Additive
            var additive = Input.GetKey(KeyCode.LeftShift);

            //Perform selections
            if (!SelectionInput(additive))
                BoxSelectionInput(additive);
        }

        //Selection input
        private bool SelectionInput(bool Additive)
        {
            if (controller.Camera != null && Input.GetMouseButtonDown(0))
            {
                //Set up ray
                var ray = controller.Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //Cast for selection
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, select.value))
                {
                    var selectable = hit.collider.gameObject.GetComponent<Selectable>();
                    if (selectable != null)
                    {
                        Select(selectable, Additive);
                        return true;
                    }
                }
            }
            return false;
        }

        private void BoxSelectionInput(bool Additive)
        {
            if (controller.Camera != null)
            {
                //Begin drag
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    if (CursorCast(out hit, boxSelect))
                        boxSelection = new BoxSelection(hit.point, hit.point, controller.FlattenedRotation, innards, border, width);
                }

                //Update drag
                if (Input.GetMouseButton(0) && boxSelection != null)
                {
                    RaycastHit hit;
                    if (CursorCast(out hit, boxSelect))
                    {
                        //Update box selection
                        boxSelection.Update(hit.point);

                        //Update soft selection
                        UpdateSoftSelection();
                    }
                }

                //End drag
                if (Input.GetMouseButtonUp(0) && boxSelection != null)
                {
                    //Clear box selection
                    boxSelection.Destroy();
                    boxSelection = null;

                    //Apply soft selection to selection
                    ApplySoftSelection(Additive);
                }
            }
        }

        //Selection
        public void ClearSelection()
        {
            if (Selection != null)
            {
                //Tell selectables they're no longer selected
                foreach (var selectable in Selection)
                    selectable.Selected = false;

                //Clear selection
                Selection.Clear();
            }
        }

        public void Select(Selectable Selectable, bool Additive)
        {
            //Initialize
            if (Selection == null) Selection = new List<Selectable>();
            else if (!Additive) ClearSelection();

            //Tell selectable it's selected
            Selectable.Selected = true;

            //Add to selection
            Selection.Add(Selectable);
        }

        public void ClearSoftSelection()
        {
            if (softSelection != null)
            {
                //Tell selectables they're no longer selected
                foreach (var selectable in softSelection)
                    selectable.Selected = false;

                //Clear soft selection
                softSelection.Clear();
            }
        }

        public void UpdateSoftSelection()
        {
            if (boxSelection != null && available != null)
            {
                //Clear old soft selection
                ClearSoftSelection();

                //Initialize soft selection
                if (softSelection == null) softSelection = new List<Selectable>();

                //Check all available selectables
                foreach (var selectable in available)
                    if (boxSelection.Contains(selectable.transform.position))
                    {
                        //Tell selectable it's selected
                        selectable.Selected = true;

                        //Add to selection
                        softSelection.Add(selectable);
                    }
            }
            
        }

        public void ApplySoftSelection(bool Additive)
        {
            if (softSelection != null)
            {
                //Initialize
                if (Selection == null) Selection = new List<Selectable>();
                else if (!Additive) ClearSelection();

                //Add soft selection to selection
                foreach (var selectable in softSelection)
                {
                    //Tell selectable it's selected
                    selectable.Selected = true;

                    //Add to selection
                    Selection.Add(selectable);
                }

                //Clear soft selection
                softSelection.Clear();
            }
        }

        //Utility
        private bool CursorCast(out RaycastHit hit, LayerMask Layers)
        {
            //Set up ray
            var ray = controller.Camera.ScreenPointToRay(Input.mousePosition);

            //Cast for selection
            return Physics.Raycast(ray, out hit, Mathf.Infinity, Layers.value);
        }
    }

    public class BoxSelection
    {
        private readonly ProjectionRenderer bottom;
        private readonly ProjectionRenderer core;
        private Vector3 end;
        private readonly ProjectionRenderer left;
        private readonly ProjectionRenderer right;

        private readonly Transform selection;

        //Core
        private Vector3 start;

        private readonly ProjectionRenderer top;

        //Display
        private readonly float width;

        public BoxSelection(Vector3 StartPosition, Vector3 EndPosition, Quaternion Orientation, ProjectionRenderer Innards, ProjectionRenderer Border, float Width)
        {
            //Setup core object
            selection = new GameObject("Box Selection").transform;
            selection.rotation = Orientation;
            selection.position = StartPosition;

            //Set positions
            start = selection.InverseTransformPoint(StartPosition);
            end = selection.InverseTransformPoint(EndPosition);

            //Set width
            width = Width;

            //Request display projections
            var pool = ProjectionPool.GetPool(0);
            core = pool.Request(Innards);
            left = pool.Request(Border);
            right = pool.Request(Border);
            top = pool.Request(Border);
            bottom = pool.Request(Border);

            //Parent display
            core.transform.SetParent(selection);
            left.transform.SetParent(selection);
            right.transform.SetParent(selection);
            top.transform.SetParent(selection);
            bottom.transform.SetParent(selection);

            //Update displays
            UpdateDisplays();
        }

        //Properties
        public float XMin
        {
            get { return Mathf.Min(start.x, end.x); }
        }

        public float XMax
        {
            get { return Mathf.Max(start.x, end.x); }
        }

        public float ZMin
        {
            get { return Mathf.Min(start.z, end.z); }
        }

        public float ZMax
        {
            get { return Mathf.Max(start.z, end.z); }
        }

        public void Destroy()
        {
            //Return projection renderers
            core.Destroy();
            left.Destroy();
            right.Destroy();
            top.Destroy();
            bottom.Destroy();

            //Destroy selection object
            Object.Destroy(selection.gameObject);
        }

        public void Update(Vector3 Position)
        {
            //Set end point
            end = selection.InverseTransformPoint(Position);

            //Update displays
            UpdateDisplays();
        }

        public void UpdateDisplays()
        {
            //Cache values
            var xMin = XMin;
            var xMax = XMax;
            var zMin = ZMin;
            var zMax = ZMax;

            //Update our displays
            UpdateDisplay(core.transform, xMin, xMax, zMin, zMax);
            UpdateDisplay(right.transform, xMax - width, xMax, zMin, zMax);
            UpdateDisplay(left.transform, xMin, xMin + width, zMin, zMax);
            UpdateDisplay(top.transform, xMin, xMax, zMax - width, zMax);
            UpdateDisplay(bottom.transform, xMin, xMax, zMin, zMin + width);
        }

        public void UpdateDisplay(Transform Display, float XMin, float XMax, float ZMin, float ZMax)
        {
            //Rotation
            Display.rotation = selection.rotation * Quaternion.Euler(90,0,0);

            //Position
            Display.localPosition = new Vector3((XMin + XMax) / 2, 0, (ZMin + ZMax) / 2);

            //Scale
            Display.localScale = new Vector3(Mathf.Max(XMax - XMin, 0.01f), Mathf.Max(ZMax - ZMin, 0.01f), 100);
        }

        public bool Contains(Vector3 Point)
        {
            var localPoint = selection.InverseTransformPoint(Point);
            if (localPoint.x > XMin && localPoint.x < XMax && localPoint.z > ZMin && localPoint.z < ZMax) return true;
            return false;
        }
    }
}