#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class ParticleDissipate : MonoBehaviour
    {
        private ParticleSystem partSystem;

        private void Start()
        {
            partSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (partSystem != null && !partSystem.IsAlive()) Destroy(gameObject);
        }
    }
}