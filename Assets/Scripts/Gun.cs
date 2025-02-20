using System.Collections;
using EZCameraShake;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun")]
public class Gun : Weapon
{
    [SerializeField] private bool penetratingShots = false;
    [SerializeField] private int maxPenetrations = 5;
    [SerializeField] private float sprayIncreaseRate = 5f;
    private float currentSpray = 0f;

    private void OnEnable()
    {
        currentSpray = 0f;
    }

    public override void TriggerWeapon(Transform cam, Animator anim, float speedIncrease, PlayerInventory inventory)
    {
        if(!runtimeData.isReloading && runtimeData.currentAmmo > 0)
        {
            if (canShoot)
            {
                currentSpray = Mathf.Min(currentSpray + sprayIncreaseRate * Time.deltaTime, spray);

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

    public override void TriggerRelease(Transform cam, Animator anim, float speedIncrease, PlayerInventory inventory)
    {
        currentSpray = 0f;
    }

    public IEnumerator GetReadyToShoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private void Shoot(Transform cam)
    {
        Vector3 shootDirection = cam.forward;
        
        if (currentSpray > 0)
        {
            shootDirection += new Vector3(
                Random.Range(-currentSpray, currentSpray),
                Random.Range(-currentSpray, currentSpray),
                Random.Range(-currentSpray, currentSpray)
            );
            shootDirection.Normalize();
        }

        if (!penetratingShots)
        {
            if (Physics.Raycast(cam.position, shootDirection, out RaycastHit hit, range, allLayers))
            {
                Instantiate(particles, hit.point, Quaternion.identity);
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.Damage(damage, hit.collider, 1);
                }
            }
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(cam.position, shootDirection, range, allLayers);
        if (hits.Length == 0) return;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        
        int enemiesHit = 0;
        foreach (RaycastHit hit in hits)
        {
            Instantiate(particles, hit.point, Quaternion.identity);
            
            if (enemiesHit < maxPenetrations && hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(damage, hit.collider, 1);
                enemiesHit++;
            }
            
            if (enemiesHit >= maxPenetrations)
            {
                break;
            }
        }
    }

    public void StartReloading(Animator anim)
    {
        InitializeRuntimeData();
        if (runtimeData.isReloading || runtimeData.currentAmmo == maxAmmo) return;
        runtimeData.isReloading = true;
        currentSpray = 0f;
        Debug.Log($"Spray Reset on Reload: {currentSpray:F4}");
    }

    public override void FinishReloading()
    {
        InitializeRuntimeData();
        runtimeData.isReloading = false;
    }
}