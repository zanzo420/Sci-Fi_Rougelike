#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class TestTrigger : MonoBehaviour
    {
        public float delay = 1;

        private float timeCode;
        private float timeElapsed;
        public Trap[] traps;

        private void OnTriggerStay(Collider other)
        {
            //Layer & duplicate check
            if (other.GetComponent<Selectable>())
            {
                //Update elapsed time
                if (Time.timeSinceLevelLoad - 1 > timeCode) timeElapsed = 0;
                else timeElapsed += Time.fixedDeltaTime;

                //Update time code
                timeCode = Time.timeSinceLevelLoad;

                //Trigger
                if (timeElapsed > delay)
                    foreach (var trap in traps)
                        trap.Trigger();
            }
        }
    }
}