#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class HeightDestructor : MonoBehaviour
    {
        public float height = -10;

        private void Update()
        {
            if (transform.position.y < height)
                Destroy(gameObject);
        }
    }
}