using System.Collections;
using EZCameraShake;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon")]
public class MeleeWeapon : Weapon
{
    public int damage;
    public float range;
    public float coolDown = 1;
    private bool canHit = true;
    public CameraShakeSettings settings;

    private float holdSpeed = 1;

    private void Awake()
    {
        canHit = true;
        holdSpeed = 1;
    }

    public override void TriggerWeapon(Transform cam, Animator animator, float speedIncrease)
    {
        if(!canHit) return;
        holdSpeed += Time.deltaTime;
        holdSpeed = Mathf.Clamp(holdSpeed, 1, 2);
        CameraShaker.Instance.ShakeOnce(holdSpeed / 4, holdSpeed * 2, settings.fadeIn, settings.fadeOut);
    }

    public override void TriggerRelease(Transform cam, Animator animator, float speedIncrease)
    {
        animator.SetTrigger("Trigger"); 
        animator.speed = holdSpeed;
          
        CameraShaker.Instance.ShakeOnce(settings.magnitude, settings.roughness, settings.fadeIn, settings.fadeOut);
        CoroutineRunner.Coroutines.StartCoroutine(CoolDown(speedIncrease));
    }

    public override void TriggerAnimation(Transform cam,  Animator animator, float speedIncrease)
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, range)) 
        {
            if(hit.collider.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(damage, hit.collider, 1);
            }
        }

        animator.speed = 1;
        holdSpeed = 1;
    }

    IEnumerator CoolDown(float speedIncrease)
    {
        canHit = false;
        yield return new WaitForSeconds(coolDown / speedIncrease);
        canHit = true;
    }
}
