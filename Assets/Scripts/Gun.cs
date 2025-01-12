using System.Collections;
using EZCameraShake;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun")]
public class Gun : Weapon
{
    public int damage;
    public float range;
    public int bulletsPerShot;
    public float spray;
    public float shootCooldown;
    public float reloadTime = 1;

    public int magazineSize;
    public int maxAmmo;
    public GameObject particles;

    public CameraShakeSettings shakeSettings;
    public string shootSound = "ShootSoundName";
    public string reloadSound = "ReloadSoundName";

    private float nextTimeToShoot;
    internal GunRuntimeData runtimeData;

    public void InitializeRuntimeData()
    {
        // Only initialize if runtimeData is null
        if (runtimeData == null)
        {
            runtimeData = new GunRuntimeData(magazineSize);
        }
        
        runtimeData.currentAmmo = magazineSize;
        nextTimeToShoot = 0;
    }

    public override void TriggerWeapon(Transform cam, Animator anim, float speedIncrease)
    {
        if(!runtimeData.isReloading && runtimeData.currentAmmo > 0)
        {
            if (Time.time >= nextTimeToShoot)
            {
                nextTimeToShoot = Time.time + shootCooldown;

                for (int i = 0; i < bulletsPerShot; i++)
                {
                    Shoot(cam);
                }

                runtimeData.currentAmmo--;
                anim.SetTrigger("Shoot");
                AudioManager.instance.PlaySound(shootSound, 1, 1.3f);
                CameraShaker.Instance.ShakeOnce(shakeSettings.magnitude, shakeSettings.roughness, shakeSettings.fadeIn, shakeSettings.fadeOut);
            }
        }
    }

    private void Shoot(Transform cam)
    {
        Vector3 shootDirection = cam.forward + new Vector3(
            Random.Range(-spray, spray),
            Random.Range(-spray, spray),
            Random.Range(-spray, spray)
        );

        if (Physics.Raycast(cam.position, shootDirection, out RaycastHit hit, range, allLayers))
        {
            Instantiate(particles, hit.point, Quaternion.identity);

            if (hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(damage, hit.collider, 1);
            }
        }
    }

    public void StartReloading(Animator anim)
    {
        InitializeRuntimeData();

        if (runtimeData.isReloading || runtimeData.currentAmmo == magazineSize) return;

        runtimeData.isReloading = true;
        anim.SetTrigger("Reload");
        AudioManager.instance.PlaySound(reloadSound, 1, 1.2f);
    }

    public void FinishReloading()
    {
        InitializeRuntimeData();

        

        runtimeData.isReloading = false;
    }
}

[System.Serializable]
public class GunRuntimeData
{
    public int currentAmmo;
    public bool isReloading;

    public GunRuntimeData(int magazineSize)
    {
        currentAmmo = magazineSize; // Set to magazine size initially
        isReloading = false;
    }
}
