#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class CollisionSpawner : MonoBehaviour
    {
        private int colliderIndex;
        public GameObject[] colliders;
        public LayerMask layers;
        public Transform parent;

        private void Update()
        {
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layers.value))
                SpawnCollider(hit.point + Vector3.up * 10);
        }

        public void SpawnCollider(Vector3 Position)
        {
            if (colliders != null && colliders.Length > 0)
            {
                //Spawn current collider
                if (colliders[colliderIndex] != null)
                {
                    var col = Instantiate(colliders[colliderIndex], Position, Quaternion.identity, parent);
                    col.name = "Collider";

                    col.GetComponent<Rigidbody>().velocity = Vector3.down * 4;
                }
                //Iterate to next collider
                colliderIndex = colliderIndex < colliders.Length - 1 ? colliderIndex + 1 : 0;
            }
        }
    }
}