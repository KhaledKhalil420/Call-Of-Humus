using UnityEngine;

public interface IDamagable
{
    public void Damage(float damage, Collider collider, float Knockback);
}
