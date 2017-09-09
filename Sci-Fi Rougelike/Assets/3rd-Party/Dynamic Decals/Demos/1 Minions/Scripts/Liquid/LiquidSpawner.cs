#region

using System.Collections;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class LiquidSpawner : MonoBehaviour
    {
        public Vector3 direction;

        [Header("GameObject")]
        public GameObject liquid;

        public Transform liquidParent;

        [Header("Positioning/Velocity")]
        public Vector3 offset;

        [Header("Rate")]
        public float spawnRate;

        public float speed = 4;
        public float spread = 0.4f;

        private void OnEnable()
        {
            StartCoroutine(SpawnLiquid());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator SpawnLiquid()
        {
            if (liquid != null)
                while (true)
                {
                    var spawn = Instantiate(liquid, transform.position + transform.rotation * offset, Quaternion.identity, liquidParent);
                    spawn.GetComponent<Rigidbody>().velocity = Vector3.Lerp(transform.rotation * direction.normalized, Random.insideUnitSphere, spread) * speed;

                    yield return new WaitForSeconds(1 / Mathf.Max(0.001f, spawnRate));
                }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position + transform.rotation * offset, 0.2f);
        }
    }
}