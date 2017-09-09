#region

using LlockhamIndustries.Decals;
using UnityEngine;

#endregion

namespace LlockhamIndustries.Misc
{
    //Hit Scan Weapon Controller
    public class HitScanWeaponController : WeaponController
    {
        public float hitScanFireRate = 1;

        [Header("Hitscan Fire")]
        public RayPrinter printer;

        public override void UpdateWeapon()
        {
            base.UpdateWeapon();
            Fire();
        }

        private void Fire()
        {
            if (timeToFire == 0)
                if ((primary || secondary) && printer != null)
                {
                    var rayPosition = cameraController.transform.position;
                    var rayDirection = cameraController.transform.forward;

                    var ray = new Ray(rayPosition, rayDirection);
                    printer.PrintOnRay(ray, 100, cameraController.transform.up);

                    //Apply recoil
                    if (controller != null) controller.ApplyRecoil(120, 0.2f);

                    timeToFire = 1 / hitScanFireRate;
                }
        }
    }
}