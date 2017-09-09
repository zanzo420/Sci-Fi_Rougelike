#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public Transform parent;
        public GameObject projectile;
        public float spawnRate = 60;

        public Vector3 spawnVelocity;
        public float spread = 0.3f;

        //Backing fields
        private float timeToSpawn;

        //Generic methods
        private void Start()
        {
            if (parent == null) parent = transform;
        }

        private void Update()
        {
            //Update time to fire
            timeToSpawn = Mathf.Clamp(timeToSpawn - Time.deltaTime, 0, Mathf.Infinity);

            //Spawn
            if (timeToSpawn == 0)
            {
                var projectileDirection = Vector3.Slerp(spawnVelocity, Random.insideUnitSphere.normalized * spawnVelocity.magnitude, spread / 10);
                var projectileRotation = Quaternion.LookRotation(projectileDirection, transform.forward);

                //Spawn projectile
                var spawn = Instantiate(projectile, transform.position, projectileRotation, parent);
                spawn.name = "Ray";

                //Setup initial velocity
                var spawnbody = spawn.GetComponent<Rigidbody>();
                spawnbody.AddForce(projectileDirection, ForceMode.VelocityChange);

                timeToSpawn = 1 / spawnRate;
            }
        }
    }
}