#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [ExecuteInEditMode]
    public class FirstPersonCharacterController : MonoBehaviour
    {
        //Backing fields
        private Rigidbody attachedRigidbody;

        [Header("Camera")]
        public Camera cameraControlled;

        public Vector3 cameraOffset = new Vector3(0, 0.6f, 0);

        private Vector3 cameraRotation;
        public float cameraSmooth = 0.2f;
        private Vector3 cameraVelocity;
        private CapsuleCollider capsuleCollider;
        private int collisions;

        [Header("Jump")]
        public float jumpAcceleration = 1;

        private bool jumpInput;
        private Vector2 lookDelta;

        [Header("Look")]
        public float lookSensitivity = 3f;

        [Header("Move")]
        public float moveAcceleration = 0.2f;

        private Vector3 moveDelta;
        public float moveSpeed = 8;

        private float recoil;
        private float recoilDuration;
        private float recoilVelocity;

        [Header("Weapon")]
        public WeaponController weapon;

        //Properties
        public bool Grounded { get; private set; }

        //Generic methods
        private void Awake()
        {
            //Grab our components
            attachedRigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();

            if (Application.isPlaying)
            {
                //Lock Cursor
                Cursor.lockState = CursorLockMode.Confined;

                //Hide Cursor
                Cursor.visible = false;
            }
        }

        private void OnEnable()
        {
            if (cameraControlled == null) cameraControlled = Camera.main;
            cameraRotation = cameraControlled.transform.rotation.eulerAngles;
        }

        private void Update()
        {
            //Look Input
            lookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            //Move Input
            moveDelta = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            //Jump Input
            if (Input.GetKey(KeyCode.Space)) jumpInput = true;
            else jumpInput = false;
        }

        //Physics methods
        private void FixedUpdate()
        {
            //Update character rotation
            var characterRotation = transform.rotation.eulerAngles;
            characterRotation.y += lookDelta.x * lookSensitivity;
            transform.rotation = Quaternion.Euler(characterRotation);

            //Get velocity
            var velocity = attachedRigidbody.velocity;

            //Update horizontal velocity
            var goalAcceleration = transform.rotation * moveDelta.normalized * moveAcceleration;
            velocity.x += goalAcceleration.x;
            velocity.z += goalAcceleration.z;

            //Clamp to max speed
            var horizontalVelocity = new Vector2(velocity.x, velocity.z);
            if (horizontalVelocity.magnitude > moveSpeed)
            {
                velocity.x *= moveSpeed / horizontalVelocity.magnitude;
                velocity.z *= moveSpeed / horizontalVelocity.magnitude;
            }

            //Grounded
            Grounded = CheckGrounded();

            //Jumping
            if (jumpInput && Grounded)
                velocity.y += jumpAcceleration;

            //Set velocity
            attachedRigidbody.velocity = velocity;

            //Update camera rotation
            cameraRotation.x -= lookDelta.y * lookSensitivity;
            cameraRotation.y += lookDelta.x * lookSensitivity;
            cameraRotation.z = 0;

            //Clamp looking too high/low
            if (cameraRotation.x < 200) cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);

            //Update recoil
            recoil = Mathf.SmoothDamp(recoil, 0, ref recoilVelocity, recoilDuration);
            var RecoiledRotation = cameraRotation;
            RecoiledRotation.x -= recoil;

            cameraControlled.transform.rotation = Quaternion.Euler(RecoiledRotation);

            //Update camera position
            cameraControlled.transform.position = Vector3.SmoothDamp(cameraControlled.transform.position, transform.TransformPoint(cameraOffset), ref cameraVelocity, cameraSmooth);

            //Update weapon - Called here instead of within its own FixedUpdate because we need to guarentee it's not updated until after the camera position has been
            if (weapon != null) weapon.UpdateWeapon();
        }

        private void OnCollisionEnter(Collision collision)
        {
            collisions++;
        }

        private void OnCollisionExit(Collision collision)
        {
            collisions--;
        }

        private bool CheckGrounded()
        {
            return collisions > 0 && Physics.Raycast(transform.position, -Vector3.up, capsuleCollider.bounds.extents.y * 1.4f);
        }

        //Recoil
        public void ApplyRecoil(float RecoilStrength, float RecoilDuration)
        {
            recoilDuration = RecoilDuration;
            recoilVelocity += RecoilStrength;
        }
    }
}
