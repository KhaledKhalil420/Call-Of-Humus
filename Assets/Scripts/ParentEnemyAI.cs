using System;
using UnityEngine;
using UnityEngine.AI;

public class ParentEnemyAI : MonoBehaviour, IDamagable
{
    public EnemySettings settings;
    public NavMeshAgent agent;
    private Transform target;

    public GameManager manager;

    private bool isDead = false;

    #region Parent Methods

    private void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        if (target != null)
        {
            FollowTarget();
        }
        else
        {
            target = PlayerManager.instance.GetClosestPlayer(transform.position);
        }

        UpdateF();
    }

    private void FollowTarget()
    {
        agent.SetDestination(target.position);
    }

    public void Damage(float damage, Collider collider, float knockback)
    {
        if (isDead) return;

        // Play damage sound
        AudioManager.instance.PlaySound("Enemy_Damage");

        // Handle damage multipliers
        if (collider == settings.headCollider || collider == settings.ballsCollider)
        {
            if(settings.oneShotCrit)
            settings.health -= damage * 100;

            else
            settings.health -= damage * 4;
        }
        else
        {
            settings.health -= damage;
        }

        // Trigger death if health is depleted
        if (settings.health <= 0)
        {
            isDead = true;
            manager.spawnedEnemies--;
            PlayerManager.instance.ChangeMoney(settings.pointWorth);
            OnDeath();
        }

        OnDamage();
    }

    #endregion

    #region Children Methods

    public virtual void OnDamage()
    {
    }

    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    public virtual void UpdateF()
    {
    }

    #endregion
}


[Serializable]
public class EnemySettings
{
    public enum ChaseState { chasing }
    public enum HeldWeaponType { melee, ranged }

    public LayerMask playerLayer;
    public Animator animator;

    [Header("Weapon settings")]
    public Weapon heldWeapon;
    public HeldWeaponType weaponType;

    [Header("Health settings")]
    public float health;
    public float pointWorth = 30;
    public bool oneShotCrit = false;

    [Header("Body Parts")]
    public Collider bodyCollider;
    public Collider headCollider;
    public Collider ballsCollider;

    [Header("Debugging")]
    public bool showGizmos;
}