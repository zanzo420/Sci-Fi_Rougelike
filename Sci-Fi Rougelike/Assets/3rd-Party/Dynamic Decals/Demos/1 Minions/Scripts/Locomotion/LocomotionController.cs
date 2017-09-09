#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Locomotion))]
    public class LocomotionController : MonoBehaviour
    {
        private bool balanced;

        public float balancedSpeed = 0.5f;

        //Requisites
        public GenericCameraController cameraController;

        //Calculation Variables
        private Locomotion locomotion;

        private float movementSpeed;
        private Vector3 movementVector;
        private Plane plane = new Plane(Vector3.up, 0);
        public float sprintSpeed = 1.6f;

        //Movement
        public float standardSpeed = 0.8f;

        private float timeSinceDodge;

        //Generic methods
        private void Awake()
        {
            locomotion = GetComponent<Locomotion>();
        }

        private void Update()
        {
            MovementSpeedInput();
            MovementInput();
            BalanceInput();
        }

        //Movement
        private void MovementSpeedInput()
        {
            movementSpeed = standardSpeed;

            //Check our BalanceMode
            if (!balanced)
            {
                //Check our Current movement type
                if (Input.GetKey(KeyCode.LeftShift) && movementVector.magnitude > 0)
                    movementSpeed = sprintSpeed;
            }
            else
            {
                movementSpeed = balancedSpeed;
            }
        }

        private void MovementInput()
        {
            movementVector = Vector3.zero;

            if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && cameraController != null)
            {
                //Models Exported as Z Backward so directions reversed to accomidate..
                var MovementDirection = Vector3.zero;
                MovementDirection -= cameraController.Forward * Input.GetAxisRaw("Vertical");
                MovementDirection -= cameraController.Right * Input.GetAxisRaw("Horizontal");

                var normalizedSpeed = Mathf.Max(Mathf.Abs(MovementDirection.x), Mathf.Abs(MovementDirection.z));
                movementVector = MovementDirection.normalized * normalizedSpeed;
            }

            //Move our Unit
            locomotion.Movement = movementVector * movementSpeed;
        }

        private void BalanceInput()
        {
            //Check if balanced
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                balanced = true;
            else
                balanced = false;

            if (balanced)
            {
                //Lock rotation towards mouse
                if (cameraController == null)
                {
                    Debug.Log("No Camera Controller Assigned! Please assign a valid camera controller.");
                    return;
                }

                var playerTargetRay = cameraController.GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
                float distToPlane;

                if (plane.Raycast(playerTargetRay, out distToPlane))
                    locomotion.Direction = -(playerTargetRay.GetPoint(distToPlane) - transform.position).normalized;
                else
                    Debug.Log("Error Casting to Plane, Cannot Determine Cursor Location");
            }
            else
            {
                locomotion.Direction = movementVector.normalized;
            }
        }
    }
}