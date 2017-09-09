#region

using System.Collections;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class AnimatedScale : MonoBehaviour
    {
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float desiredScale = 2;

        private Vector3 initialScale;
        private float sampleTime;
        public float speed = 1;

        private void OnEnable()
        {
            StartCoroutine(Scale());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Scale()
        {
            yield return new WaitForFixedUpdate();

            initialScale = transform.localScale;

            while (sampleTime < 1)
            {
                //Increment time elapsed
                sampleTime = Mathf.MoveTowards(sampleTime, 1, Time.fixedDeltaTime / speed);

                //Adjust scale
                var scaleModifier = Mathf.Lerp(1, desiredScale, curve.Evaluate(sampleTime));
                transform.localScale = initialScale * scaleModifier;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}