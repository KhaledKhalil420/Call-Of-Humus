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
    private bool canShoot = true;

    public int maxAmmo;
    public GameObject particles;

    public CameraShakeSettings shakeSettings;
    public string shootSound = "ShootSoundName";
    public string reloadSound = "ReloadSoundName";

    internal GunRuntimeData runtimeData;

    public void InitializeRuntimeData()
    {
        // Only initialize if runtimeData is null
        if (runtimeData == null)
        {
            runtimeData = new GunRuntimeData(maxAmmo);
        }
        
        runtimeData.currentAmmo = maxAmmo;
        canShoot = true;
    }

    public override void TriggerWeapon(Transform cam, Animator anim, float speedIncrease)
    {
        if(!runtimeData.isReloading && runtimeData.currentAmmo > 0)
        {
            if (canShoot)
            {
                for (int i = 0; i < bulletsPerShot; i++)
                {
                    Shoot(cam);
                }

                runtimeData.currentAmmo--;
                anim.SetTrigger("Shoot");
                AudioManager.instance.PlaySound(shootSound, 1, 1.3f);
                CameraShaker.Instance.ShakeOnce(shakeSettings.magnitude, shakeSettings.roughness, shakeSettings.fadeIn, shakeSettings.fadeOut);
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

        if (runtimeData.isReloading || runtimeData.currentAmmo == maxAmmo) return;

        runtimeData.isReloading = true;
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
