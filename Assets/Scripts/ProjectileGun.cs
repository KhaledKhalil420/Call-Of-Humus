using UnityEngine;
using System.Collections;
using EZCameraShake;

[CreateAssetMenu(fileName = "New Projectile Gun")]
public class ProjectileGun : Weapon
{
    public GameObject projectile;

    public override void TriggerWeapon(Transform cam, Animator anim, float speedIncrease, PlayerInventory inventory)
    {
        if (!runtimeData.isReloading && runtimeData.currentAmmo > 0)
        {
            if (canShoot)
            {   
                //Shoot Projectile
                FireProjectile(inventory, cam);
                
                //Ammo
                runtimeData.currentAmmo--;
                
                //Audio, Animations, CameraShake
                anim.SetTrigger("Shoot");
                AudioManager.instance.PlaySound(shootSound, 1, 1.3f);
                CameraShaker.Instance.ShakeOnce(shakeSettings.magnitude, shakeSettings.roughness, shakeSettings.fadeIn, shakeSettings.fadeOut);
                
                //Use CoolDown
                CoroutineRunner.Coroutines.StartCoroutine(GetReadyToShoot());
            }
        }
    }

    public IEnumerator GetReadyToShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private void FireProjectile(PlayerInventory inventory, Transform cam)
    {
        //Reference
        WeaponObject weaponObj = inventory.currentWeaponModel.GetComponent<WeaponObject>();             

        //Instantiate
        Transform proje = Instantiate(projectile, weaponObj.gunTip.position, cam.rotation).transform;
        proje.transform.position = weaponObj.gunTip.position;
    }

    public void StartReloading()
    {
        InitializeRuntimeData();

        if (runtimeData.isReloading || runtimeData.currentAmmo == maxAmmo) return;

        runtimeData.isReloading = true;
    }

    public override void FinishReloading()
    {
        InitializeRuntimeData();
        runtimeData.isReloading = false;
    }
}