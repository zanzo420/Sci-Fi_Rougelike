#region

using LlockhamIndustries.ExtensionMethods;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class Ragdoll : MonoBehaviour
    {
        public float chunkAngle = 35;
        public GameObject chunkdoll;
        public ParticleSystem chunkParticles;

        [Header("Chunkdoll Triggers")]
        public float chunkVelocity = 50;

        [Header("GameObjects")]
        public GameObject ragdoll;

        [Header("Particles")]
        public ParticleSystem ragParticles;

        [Header("Ragdoll Triggers")]
        public float ragVelocity = 10;

        [Header("Layers")]
        public LayerMask triggerLayers;

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                //Grab rigidbody
                var rigidbody = contact.otherCollider.GetComponent<Rigidbody>();

                if (rigidbody != null && rigidbody.velocity.magnitude > chunkVelocity && triggerLayers.Contains(rigidbody.gameObject.layer))
                {
                    if (rigidbody.velocity.magnitude > chunkVelocity && Vector3.Angle(rigidbody.velocity, contact.normal) < chunkAngle)
                    {
                        //Trigger chunkdoll
                        TriggerChunkdoll(rigidbody.mass, rigidbody.velocity);

                        //Grab first chunk
                        var chunk = chunkdoll.transform.GetChild(0);

                        //Spawn & play particles
                        SpawnParticles(chunkParticles, contact.point, contact.normal, chunk);
                        return;
                    }
                    if (rigidbody.velocity.magnitude > ragVelocity)
                    {
                        //Trigger ragdoll
                        TriggerRagdoll(rigidbody.mass, rigidbody.velocity);

                        //Spawn & play particles
                        SpawnParticles(ragParticles, contact.point, contact.normal, ragdoll.transform);
                        return;
                    }
                }
            }
        }

        private void TriggerRagdoll(float ExternalMass, Vector3 ExternalVelocity)
        {
            //Disable our collider
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            //Spawn ragdoll
            ragdoll = Instantiate(ragdoll, transform.position, transform.rotation, transform.parent);

            //Position ragdoll
            SyncTransformRecursively(ragdoll.transform, transform);

            //Calculate mass
            var currentMass = CalculateMassRecursively(ragdoll.transform);

            //Calculate velocity
            var lerp = ExternalMass / (ExternalMass + currentMass);
            var velocity = Vector3.Lerp(Vector3.zero, ExternalVelocity, lerp);

            //Set velocity
            SetVelocityRecursively(ragdoll.transform, velocity);

            //Destroy ourself
            Destroy(gameObject);
        }

        private void TriggerChunkdoll(float ExternalMass, Vector3 ExternalVelocity)
        {
            //Disable our collider
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            //Spawn chunkdoll
            chunkdoll = Instantiate(chunkdoll, transform.position, transform.rotation, transform.parent);

            //Calculate mass
            var currentMass = CalculateMassRecursively(chunkdoll.transform);

            //Calculate velocity
            var lerp = ExternalMass / (ExternalMass + currentMass);
            var velocity = Vector3.Lerp(Vector3.zero, ExternalVelocity, lerp);

            //Set velocity
            SetVelocityRecursively(chunkdoll.transform, velocity * 2);

            //Destroy ourself
            Destroy(gameObject);
        }

        private void SpawnParticles(ParticleSystem Particles, Vector3 Position, Vector3 Normal, Transform Parent)
        {
            if (Particles != null)
            {
                var p = Instantiate(Particles, Position, Quaternion.LookRotation(Normal), Parent);
                p.name = Particles.name;
                p.Play();
            }
        }

        private void SyncTransformRecursively(Transform Transform, Transform Target)
        {
            Transform.localPosition = Target.localPosition;
            Transform.localRotation = Target.localRotation;

            foreach (Transform child in Transform)
            {
                var target = Target.Find(child.name);
                SyncTransformRecursively(child, target);
            }
        }

        private float CalculateMassRecursively(Transform Transform)
        {
            float mass = 0;

            //Add rigidbody mass
            var rigidbody = Transform.GetComponent<Rigidbody>();
            if (rigidbody != null) mass += rigidbody.mass;

            //Add child rigidbodies mass
            foreach (Transform child in Transform) mass += CalculateMassRecursively(child);

            return mass;
        }

        private void SetVelocityRecursively(Transform Transform, Vector3 Velocity)
        {
            var rigidbody = Transform.GetComponent<Rigidbody>();
            if (rigidbody != null) rigidbody.velocity = Velocity;

            foreach (Transform child in Transform)
                SetVelocityRecursively(child, Velocity);
        }
    }
}