#region

using LlockhamIndustries.ExtensionMethods;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(Rigidbody))]
    public class ParticleCollisionSpawner : MonoBehaviour
    {
        public LayerMask layers;

        [Header("Pool Parent")]
        public Transform parent;

        [Header("Particle System")]
        public ParticleSystem particles;

        private Rigidbody rb;

        [Header("Conditions")]
        public float requiredVelocity = 10;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (particles != null && rb.velocity.magnitude > requiredVelocity)
                for (var i = 0; i < collision.contacts.Length; i++)
                    if (layers.Contains(collision.contacts[i].otherCollider.gameObject.layer))
                    {
                        ParticleSystem p = null;

                        if (parent != null) p = Instantiate(particles, collision.contacts[i].point, Quaternion.LookRotation(collision.contacts[i].normal), parent);
                        else p = Instantiate(particles, collision.contacts[i].point, Quaternion.LookRotation(collision.contacts[i].normal), transform);

                        p.Play();
                    }
        }
    }
}