#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class TimedDestructor : MonoBehaviour
    {
        private float t;
        public float time = 10;

        private void Update()
        {
            t += Time.deltaTime;
            if (t > time)
                Destroy(gameObject);
        }
    }
}