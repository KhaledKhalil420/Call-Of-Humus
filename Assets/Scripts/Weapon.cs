using UnityEngine;

public class Weapon : ScriptableObject
{
    public GameObject model;
    public RuntimeAnimatorController animator;
    public LayerMask allLayers;
    public string itemName;
    public Sprite sprite;

    [Header("Gun Data")]
    public int damage;
    public float range;
    public int bulletsPerShot;
    public float spray;
    public float shootCooldown;
    public float reloadTime = 1;
    internal bool canShoot = true;

    public int maxAmmo;
    public GameObject particles;

    public CameraShakeSettings shakeSettings;
    public string shootSound = "ShootSoundName";
    public string reloadSound = "ReloadSoundName";

    internal GunRuntimeData runtimeData;

    public virtual void TriggerWeapon(Transform cam, Animator animator, float speedIncrease, PlayerInventory inventory)
    {

    }

    public virtual void TriggerWeapon(Transform cam, Animator animator, float speedIncrease, PlayerInventory inventory, bool isRightWeapon)
    {

    }
    
    public virtual void TriggerRelease(Transform cam, Animator animator, float speedIncrease, PlayerInventory inventory)
    {

    }

    public virtual void TriggerWeaponOnce()
    {

    }

    public virtual void TriggerAnimation(Transform cam,  Animator animator,float speedIncrease, PlayerInventory inventory)
    {

    }

    public virtual void FinishReloading()
    {

    }

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
}

[System.Serializable]
public class GunRuntimeData
{
    public int currentAmmo;
    public bool isReloading;

    public GunRuntimeData(int magazineSize)
    {
        currentAmmo = magazineSize;
        isReloading = false;
    }
}
