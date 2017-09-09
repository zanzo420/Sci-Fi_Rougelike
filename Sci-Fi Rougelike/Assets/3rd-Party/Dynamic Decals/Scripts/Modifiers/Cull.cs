using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LlockhamIndustries.Decals
{
    /**
    * Culls attached projection (destroy or return to pool) once it's no longer visible by any cameras. Useful for cleaning up your scene without the player noticing.
    * Designed to be printed with your projections. Attach to your prefab and enable print behaviours on your printer. You can turn this on or off by enabling and disbaling this component respectively.
    */
    [RequireComponent(typeof(ProjectionRenderer))]
    public class Cull : MonoBehaviour
    {
        //Inspector variables
        /**
        * How long the projection has to be off screen before it's culled. 0 will cull the projection the second it's no longer visible.
        */
        public float cullTime = 4;
        /**
        * How often we check if the projection is visible. There's no point checking visibility 60+ times a second. 0.05 will check 20 times a second, 0.5, twice a second and 2 once every 2 seconds. 
        */
        public float updateRate = 0.05f;

        //Backing fields
        private ProjectionRenderer projection;
        private bool executing;

        private void Awake()
        {
            //Grab our projection
            projection = GetComponent<ProjectionRenderer>();
        }
        private void OnEnable()
        {
            //Begin cull
            InvokeCull();
        }
        private void OnDisable()
        {
            EndCull();
        }

        public void InvokeCull()
        {
            StartCoroutine(CullRoutine(projection, updateRate));
        }
        public void EndCull()
        {
            //Stop Fade
            StopAllCoroutines();
        }

        public IEnumerator CullRoutine(ProjectionRenderer Projection, float UpdateRate)
        {
            //Execution check
            if (executing) yield break;
            else executing = true;

            //Perform fade
            float timeElapsed = 0;
            while (timeElapsed < cullTime)
            {
                //Update time elapsed
                timeElapsed += UpdateRate;

                //Reset time elapsed if visible
                if (Projection.Renderer.isVisible) timeElapsed = 0;

                yield return new WaitForSeconds(UpdateRate);
            }

            //Destroy projection
            Projection.Destroy();

            //No longer executing
            executing = false;
        }
    }
}