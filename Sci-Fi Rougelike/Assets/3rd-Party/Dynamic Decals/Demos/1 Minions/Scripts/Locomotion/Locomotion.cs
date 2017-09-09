#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class Locomotion : MonoBehaviour
    {
        //Inspector
        public float acceleration = 0.2f;

        //Backing fields
        private Animator anim;

        private Vector2 locomotion;

        private float locomotionStrength;
        private float locomotionStrengthVelocity;
        private Vector3 moveDirectionVelocity;
        private Vector3 movement;
        private float moveSpeedVelocity;
        private float previousLocomotionStrength;

        private Vector3 previousMovement;
        private float previousMoveSpeed;

        private float rotation;
        private Vector3 rotationDirection;
        public float rotationSpeed = 0.2f;
        private float rotationVelocity;

        //Properties
        public Vector3 Movement
        {
            set { movement = value; }
        }

        public Vector3 Direction
        {
            set { rotationDirection = value; }
        }

        //Generic Methods
        protected void Awake()
        {
            //Grab components
            anim = GetComponent<Animator>();

            //set rotation forward
            rotation = Quaternion.LookRotation(transform.forward).eulerAngles.y;
        }

        protected void FixedUpdate()
        {
            UpdateRotation();
            UpdateLocomotionDirection();
            UpdateLocomotionStrength();
        }

        //Rotation
        public void UpdateRotation()
        {
            var goalRotation = rotation;

            if (rotationDirection != Vector3.zero)
            {
                goalRotation = Quaternion.LookRotation(rotationDirection).eulerAngles.y;

                //Reduce our rotationSpeed as the unit moves faster
                var MoveSpeedModifier = 1 + Mathf.Clamp01(locomotion.magnitude - 1);
                var RotationSpeed = 1 / rotationSpeed * MoveSpeedModifier;

                rotation = Mathf.SmoothDampAngle(rotation, goalRotation, ref rotationVelocity, RotationSpeed * 0.02f);
            }
            else
            {
                rotationVelocity = 0;
            }

            //Set our rotation
            transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
        }

        //Movement
        public void UpdateLocomotionDirection()
        {
            //Smooth our Movement Direction
            var Movement = Vector3.SmoothDamp(previousMovement, movement, ref moveDirectionVelocity, 1 / acceleration * 0.02f);
            previousMovement = Movement;

            //Get the direction as an angle from forward, in Degrees, from -180 to 180..
            var directionAngle = Vector3.Angle(transform.forward, Movement.normalized);
            if (Mathf.Sign(Vector3.Dot(transform.right, Movement.normalized)) < 0)
                directionAngle = 360 - directionAngle;
            directionAngle = directionAngle * (Mathf.PI / 180);

            //Convert This Angle into seperate X and Y vectors..
            locomotion.x = Mathf.Sin(directionAngle);
            locomotion.y = Mathf.Cos(directionAngle);

            //Multiply Locomotion by the MovementSpeed of the character..
            locomotion *= Movement.magnitude;

            //The Sideward Movement of the character can only reach 0.5f, so if it's over, normalise our locomotion until it's at 0.5f
            if (locomotion.x > 0.5f)
            {
                var locNormalise = 0.5f / locomotion.x;
                locomotion *= locNormalise;
            }

            anim.SetFloat("X", locomotion.x);
            anim.SetFloat("Y", locomotion.y);
        }

        public void UpdateLocomotionStrength()
        {
            //General Movement
            var LocomotionStrengthMovement = Mathf.Clamp01(movement.magnitude * 2);

            //Rotating on the Spot
            var LocomotionStrengthRotation = Mathf.Clamp(Mathf.Abs(rotationVelocity / 400), 0, 1);

            //Determine Locomotion Strength and Smooth
            locomotionStrength = Mathf.Max(LocomotionStrengthMovement, LocomotionStrengthRotation);
            locomotionStrength = Mathf.SmoothDamp(previousLocomotionStrength, locomotionStrength, ref locomotionStrengthVelocity, 1 / acceleration * 0.02f);
            previousLocomotionStrength = locomotionStrength;

            anim.SetFloat("Locomote", locomotionStrength);
        }
    }
}