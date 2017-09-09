#region

using LlockhamIndustries.ExtensionMethods;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class Bleeder : MonoBehaviour
    {
        public int bleedLimit = 12;
        public int bleedRate = 2;
        public GameObject prefab;

        public LayerMask triggerLayers;
        public float triggerVelocity = 10;

        private bool Valid
        {
            get
            {
                if (prefab == null) return false;
                if (prefab.GetComponent<Collider>() == null) return false;
                if (prefab.GetComponent<Rigidbody>() == null) return false;
                if (prefab.GetComponent<Blood>() == null) return false;

                return true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Valid)
                for (var i = 0; i < Mathf.Min(bleedLimit, collision.contacts.Length); i++)
                {
                    //Rigidbody check
                    var r = collision.contacts[i].otherCollider.GetComponent<Rigidbody>();
                    if (r == null || r.velocity.magnitude <= triggerVelocity) continue;

                    //Blood check
                    var b = collision.contacts[i].otherCollider.GetComponent<Blood>();
                    if (b != null && b.source == this) continue;

                    //Layer check
                    if (!triggerLayers.Contains(collision.contacts[i].otherCollider.gameObject.layer)) continue;

                    //Bleed
                    Bleed(collision.contacts[i].point, collision.contacts[i].normal);
                }
        }

        private void Bleed(Vector3 Point, Vector3 Normal)
        {
            //Grab blood collider bounds
            var bounds = prefab.GetComponent<Collider>().bounds;
            var offset = 1.5f * Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);

            //Calculate position to prevent self collision
            var position = Point + Normal * offset;

            for (var i = 0; i < bleedRate; i++)
            {
                //Calculate direction
                var direction = (Normal + Random.onUnitSphere * 0.2f).normalized;

                //Spawn droplets
                SpawnDroplet(position, direction);
            }
            
        }

        private void SpawnDroplet(Vector3 Point, Vector3 Velocity)
        {
            //Spawn
            var b = Instantiate(prefab.gameObject, Point, Quaternion.identity).GetComponent<Blood>();
            b.source = this;

            //Grab rigidbody & set velocity
            var rigidbody = b.GetComponent<Rigidbody>();
            rigidbody.velocity = Velocity;
        }
    }
}