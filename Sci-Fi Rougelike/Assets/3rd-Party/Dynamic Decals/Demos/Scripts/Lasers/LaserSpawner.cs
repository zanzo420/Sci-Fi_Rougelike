#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    public class LaserSpawner : MonoBehaviour
    {
        //Inspector
        public GameObject laser;

        //Backing fields
        [SerializeField]
        private int laserCount = 1;

        private List<GameObject> laserPool;

        //Laser pool
        private List<GameObject> lasers;

        [SerializeField]
        private int spawnRate = 5;

        //Properties
        public int LaserCount
        {
            get { return laserCount; }
            set
            {
                laserCount = value;
                LaserCountChange();
            }
        }

        //Generic methods
        private void Awake()
        {
            lasers = new List<GameObject>();
            laserPool = new List<GameObject>();
        }

        private void Start()
        {
            LaserCountChange();
        }

        public GameObject RequestLaser()
        {
            GameObject Laser = null;

            if (laserPool.Count > 0)
            {
                //Grab our laser
                Laser = laserPool[0];

                //Enable
                Laser.SetActive(true);

                //Remove from pool
                laserPool.RemoveAt(0);
            }
            else
            {
                //Create a new laser
                Laser = Instantiate(laser, Vector3.zero, Quaternion.LookRotation(-Vector3.up, -Vector3.right), transform);
            }

            //Add to active lasers
            lasers.Add(Laser);

            return Laser;
        }

        public void ReturnLaser(GameObject laser)
        {
            //Remove from active lasers
            lasers.Remove(laser);

            //Disable
            laser.SetActive(false);

            //Move to origin
            laser.transform.position = Vector3.zero;

            //Add to pool
            laserPool.Add(laser);
        }

        //Laser count
        public void LaserCountChange()
        {
            if (Application.isPlaying)
            {
                var lasersSpawned = 0;

                //Add as required, limited by spawn rate
                while (lasers != null && lasers.Count < laserCount && lasersSpawned < spawnRate)
                {
                    RequestLaser();
                    lasersSpawned++;
                }
                //Remove as required
                while (lasers != null && lasers.Count > laserCount)
                    ReturnLaser(lasers[lasers.Count - 1]);
            }
        }
    }
}