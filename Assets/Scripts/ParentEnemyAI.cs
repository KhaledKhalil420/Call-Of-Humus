using System;
using System.Dynamic;
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

        if(settings.randomizeSpeed)
        {
            settings.startSpeed = UnityEngine.Random.Range(settings.startSpeed, settings.startSpeed * 1.3f);
            agent.speed = settings.startSpeed;
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


        // Handle damage multipliers
        if (collider == settings.headCollider || collider == settings.ballsCollider)
        {
            
            if(settings.oneShotCrit)
            settings.health -= damage * 100;

            else
            settings.health -= damage * 4;

            AudioManager.instance.PlaySound("Enemy_Headshot");
            PlayerManager.instance.TriggerHitMarker();
        }
        else
        {
            settings.health -= damage;
            AudioManager.instance.PlaySound("Enemy_Damage");
        }


        OnDamage(collider);

        // Trigger death if health is depleted
        if (settings.health <= 0)
        {
            isDead = true;
            manager.spawnedEnemies--;
            PlayerManager.instance.ChangeMoney(settings.pointWorth);
            OnDeath(collider);
        }
    }

    #endregion

    #region Children Methods

    public virtual void OnDamage(Collider collider)
    {
    }

    public virtual void OnDeath(Collider collider)
    {
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

    [Header("Seped")]
    public float startSpeed;
    public bool randomizeSpeed = false;
}