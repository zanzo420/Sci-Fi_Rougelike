#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    //Camera Controller
    public class TargetedCameraController : GenericCameraController
    {
        private Vector3 basePos;
        private Vector3 cameraVelocity;
        public AnimationCurve lookCurve;

        [Header("Look")]
        public float lookSensitivity = 0.3f;

        public float lookSpeed = 0.2f;

        private Vector3 offset;
        private Vector3 offsetVelocity;

        //Backing fields
        private Vector2 screenOffset;

        [Header("Target")]
        public Transform target;

        public float trackingSpeed = 0.1f;

        //Generic methods
        private void Update()
        {
            OffsetInput();
            RotationZoomInput();
        }

        private void LateUpdate()
        {
            ApplyPosition();
            ApplyRotationZoom();
        }

        //Targeted offset
        private void OffsetInput()
        {
            var screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
            screenOffset.x = (Input.mousePosition.x - screenCentre.x) / screenCentre.x;
            screenOffset.y = (Input.mousePosition.y - screenCentre.y) / screenCentre.y;
        }

        private void ApplyPosition()
        {
            if (target != null)
            {
                //Base position
                basePos = Vector3.SmoothDamp(basePos, target.position, ref cameraVelocity, trackingSpeed);

                //Offset position
                screenOffset = screenOffset.normalized * lookCurve.Evaluate(screenOffset.magnitude) * lookSensitivity;

                //Convert screen offset into a position offset
                var positionOffset = Forward * screenOffset.y + Right * screenOffset.x;

                //Smooth offset
                offset = Vector3.SmoothDamp(offset, positionOffset, ref offsetVelocity, lookSpeed);

                //Final Position
                transform.position = basePos + offset * zoom;
            }
        }
    }
}