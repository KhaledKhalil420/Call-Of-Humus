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
    
    public int magazineSize;
    private float nextTimeToShoot;
    private int currentAmmo;

    public GameObject particles;
    private bool canShoot;

    public CameraShakeSettings shakeSettings;
    public string sound = "ShootSoundName";

    private void OnEnable()
    {
        currentAmmo = magazineSize;
        canShoot = true;
    }

    public override void TriggerWeapon(Transform cam, Animator anim, float speedIncrease)
    {
        if (canShoot)
        {
            nextTimeToShoot = Time.time + shootCooldown;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                Shoot(cam);
            }

            currentAmmo--;
            anim.SetTrigger("Shoot");
            AudioManager.instance.PlaySound(sound, 1, 1.3f);
            CoroutineRunner.Coroutines.StartCoroutine(CoolDown(speedIncrease));
            CameraShaker.Instance.ShakeOnce(shakeSettings.magnitude, shakeSettings.roughness, shakeSettings.fadeIn, shakeSettings.fadeOut);
        }
    }

    private void Shoot(Transform cam)
    {
        Vector3 shootDirection = cam.forward + new Vector3(
            Random.Range(-spray, spray),
            Random.Range(-spray, spray),
            Random.Range(-spray, spray)
        );

        RaycastHit hit;
        if (Physics.Raycast(cam.position, shootDirection, out hit, range, allLayers))
        {
            Instantiate(particles, hit.point, Quaternion.identity);

            if(hit.collider.TryGetComponent(out IDamagable damagable))
                damagable.Damage(damage, hit.collider, 1);
        }
    }

    public IEnumerator CoolDown(float speedIncrease)
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown / speedIncrease);
        canShoot = true;
    }
}
