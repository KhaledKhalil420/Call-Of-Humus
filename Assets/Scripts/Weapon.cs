using UnityEngine;

public class Weapon : ScriptableObject
{
    public GameObject model;
    public RuntimeAnimatorController animator;
    public LayerMask allLayers;

    public virtual void TriggerWeapon(Transform cam, Animator animator, float speedIncrease)
    {

    }
    
    public virtual void TriggerRelease(Transform cam, Animator animator, float speedIncrease)
    {

    }

    public virtual void TriggerWeaponOnce()
    {

    }

    public virtual void TriggerAnimation(Transform cam,  Animator animator,float speedIncrease)
    {

    }
}
