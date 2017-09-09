#region

using System.Collections;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class Cannon : MonoBehaviour
    {
        [Header("References")]
        public GameObject ball;

        public Rigidbody barrel;
        public float fireRate = 0.25f;

        [Header("Firing")]
        public Vector3 offset;

        public ParticleSystem particles;

        private float timeElapsed;
        public Vector3 velocity = new Vector3(0, -10, 0);

        private void OnEnable()
        {
            //Start fire routine
            StartCoroutine(FireRoutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator FireRoutine()
        {
            while (true)
            {
                //Increment time elapsed
                timeElapsed += Time.fixedDeltaTime;

                if (timeElapsed > 1 / fireRate)
                {
                    //Fire!!
                    Fire();

                    //Reduce time elapsed
                    timeElapsed -= 1 / fireRate;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void Fire()
        {
            //Play particle effect
            if (particles != null) particles.Play();
            
            if (barrel != null && ball != null)
            {
                //Instantiate cannonball
                var cannonBall = Instantiate(ball, barrel.transform.position + barrel.transform.rotation * offset, Quaternion.identity, transform);
                var crb = cannonBall.GetComponent<Rigidbody>();

                //Calculate velocity
                var ballVelocity = barrel.transform.rotation * velocity;

                //Give cannonball velocity
                crb.velocity = ballVelocity;

                //Calculare barrel velocity
                var barrelVelocity = -ballVelocity * (crb.mass / barrel.mass);

                //Apply equal force against barrel
                barrel.velocity = barrelVelocity;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (barrel != null)
            {
                var position = barrel.transform.position + barrel.transform.rotation * offset;
                var direction = barrel.transform.rotation * velocity;

                Gizmos.DrawWireSphere(position, 0.2f);
                Gizmos.DrawRay(position, direction);
            }
        }
    }
}