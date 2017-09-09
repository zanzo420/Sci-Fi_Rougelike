#region

using System.Collections;
using LlockhamIndustries.Decals;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    [RequireComponent(typeof(ProjectionRenderer))]
    public class Goo : MonoBehaviour
    {
        private ProjectionRenderer projection;
        public GooType type;

        private void OnEnable()
        {
            //Grab projection
            projection = GetComponent<ProjectionRenderer>();

            //Register to singleton
            StartCoroutine(Register());
        }

        private void OnDisable()
        {
            //Stop register coroutine if still running
            StopAllCoroutines();

            //Deregister from singleton
            Deregister();
        }

        private IEnumerator Register()
        {
            while (!GooManager.Register(projection, type)) yield return new WaitForFixedUpdate();
        }

        private void Deregister()
        {
            GooManager.Deregister(projection, type);
        }
    }
}