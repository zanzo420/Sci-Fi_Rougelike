#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class FreeCameraController : GenericCameraController
    {
        private Vector3 cameraVelocity;
        public float maxX = 10;
        public float maxZ = 10;

        [Header("Limits")]
        public float minX = -10;

        public float minZ = -10;

        //Backing fields
        private Vector2 mousePosition;

        [Header("Movement")]
        public float movementSpeed = 0.1f;

        public float movementThreshold = 0.1f;

        //Generic methods
        private void Update()
        {
            EdgeScrollInput();
            RotationZoomInput();
        }

        private void LateUpdate()
        {
            ApplyEdgeScroll();
            ApplyRotationZoom();
        }

        //Edge scroll
        private void EdgeScrollInput()
        {
            //Mouse position
            mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
        }

        private void ApplyEdgeScroll()
        {
            //Calculate camera movement
            var movement = Vector3.zero;

            if (mousePosition.x < movementThreshold) movement -= Right * (movementThreshold - mousePosition.x) / movementThreshold * movementSpeed;
            if (1 - mousePosition.x < movementThreshold) movement += Right * (movementThreshold - (1 - mousePosition.x)) / movementThreshold * movementSpeed;
            if (mousePosition.y < movementThreshold) movement -= Forward * (movementThreshold - mousePosition.y) / movementThreshold * movementSpeed;
            if (1 - mousePosition.y < movementThreshold) movement += Forward * (movementThreshold - (1 - mousePosition.y)) / movementThreshold * movementSpeed;

            //Scale movement by zoom
            movement *= zoom / maxZoom;

            //Calculate goal position
            var goalPosition = transform.position + movement;

            //Clamp goal position
            goalPosition.x = Mathf.Clamp(goalPosition.x, minX, maxX);
            goalPosition.z = Mathf.Clamp(goalPosition.z, minZ, maxZ);

            //Position
            transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref cameraVelocity, 0.1f);
        }
    }
}
