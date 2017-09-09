#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Decals
{
    /**
    * The CollisionPrinter Component. Prints a projection under set conditions related to the collision of the object attached to this printer.
    */
    [ExecuteInEditMode]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleCollisionPrinter : Printer
    {
        private List<ParticleCollisionEvent> collisionEvents;
        private float maxparticleCollisionSize;

        private ParticleSystem partSystem;

        /**
        * Defines the percentage of particles that print projections. At 0, no particles will print, at 1, all will.
        */
        public float ratio = 1;

        /**
        * Defines the orientation of the projection relative to the surface of the collision. Velocity will orient the projection as if its up is the direction the collision object is moving in. Random will orient the projection as if its up is random.
        */
        public RotationSource rotationSource;

        private void Start()
        {
            //Grab Particle System
            partSystem = GetComponent<ParticleSystem>();

            if (Application.isPlaying)
                collisionEvents = new List<ParticleCollisionEvent>();
        }

        private void Update()
        {
            // Log Setup Warnings
            if (partSystem.collision.enabled != true)
                Debug.LogWarning("Particle system collisions must be enabled for the particle system to print decals");
            else if (partSystem.collision.sendCollisionMessages != true)
                Debug.LogWarning("Particle system must send collision messages for the particle system to print decals. This option can be enabled under the collisions menu.");
        }

        private void OnParticleCollision(GameObject other)
        {
            if (Application.isPlaying && ratio > 0)
            {
                var numCollisionEvents = partSystem.GetCollisionEvents(other, collisionEvents);

                var i = 0;
                while (i < numCollisionEvents)
                {
                    if (ratio == 1 || ratio > Random.Range(0f, 1f))
                    {
                        //Grab collision data
                        var position = collisionEvents[i].intersection;
                        var normal = collisionEvents[i].normal;
                        var surface = other.transform;

                        //Create layermask
                        var layerMask = 1 << other.layer;

                        //Calculate final position and surface normal
                        RaycastHit hit;
                        if (Physics.Raycast(position, -normal, out hit, Mathf.Infinity, layerMask))
                        {
                            position = hit.point;
                            normal = hit.normal;
                            surface = hit.collider.transform;

                            //Calculate our rotation
                            Vector3 rot;
                            if (rotationSource == RotationSource.Velocity && collisionEvents[i].velocity != Vector3.zero) rot = collisionEvents[i].velocity.normalized;
                            else if (rotationSource == RotationSource.Random) rot = Random.insideUnitSphere.normalized;
                            else rot = Vector3.up;

                            //Print
                            Print(position, Quaternion.LookRotation(-normal, rot), surface, hit.collider.gameObject.layer);
                        }
                    }
                    i++;                    
                }
            }
        }
    }
}