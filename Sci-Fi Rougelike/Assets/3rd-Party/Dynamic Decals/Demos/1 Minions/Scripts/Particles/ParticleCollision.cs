#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class ParticleCollision : MonoBehaviour
    {
        public ParticleSystem partSystem;

        private void OnCollisionEnter(Collision collision)
        {
            var ps = Instantiate(partSystem, transform.position, partSystem.transform.rotation, transform.parent).gameObject;
            ps.name = "Splash Particles";
        }
    }
}