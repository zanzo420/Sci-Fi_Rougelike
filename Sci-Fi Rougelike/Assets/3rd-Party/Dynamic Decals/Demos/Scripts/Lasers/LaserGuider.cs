#region

using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class LaserGuider : MonoBehaviour
    {
        private Vector3 goalPosition;

        //Height
        public float laserHeight = 2.2f;

        //Retarget Distance
        public float retargetDistance = 4;

        //Movement Smooth
        public float smooth = 0.6f;

        public float xMax = 20;

        //Bounds
        public float xMin = -20;

        public float zMax = 20;
        public float zMin = -20;

        //Generate Random Position within Bounds
        private Vector3 NewPosition
        {
            get
            {
                var xPosition = Random.Range(xMin, xMax);
                var zPosition = Random.Range(zMin, zMax);

                return new Vector3(xPosition, laserHeight, zPosition);
            }
        }
        //private Vector3 velocity;

        private void Update()
        {
            //Check if we are close to our goal position
            if (Vector3.Distance(transform.position, goalPosition) <= retargetDistance)
                goalPosition = NewPosition;
            //Move towards our goal position
            //transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref velocity, smooth);
            transform.position = Vector3.MoveTowards(transform.position, goalPosition, smooth * Time.deltaTime);
        }
    }
}