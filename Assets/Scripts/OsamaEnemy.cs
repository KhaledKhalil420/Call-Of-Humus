using System.Collections;
using UnityEngine;

public class OsamaEnemy : ParentEnemyAI
{
    [Header("Attack Settings")]
    public float attackRange;
    public float attackKnockback = 500;
    public float attackDamage;
    public float attackDelay = 1f;
    private bool canAttack = true;
    private Vector3 facePos;

    [Header("Effects & Sounds")]
    public GameObject death;
    public AudioSource damagedAudio;
    public Transform bombCollider;
    public GameObject explosion;
    public float explosionRadius;

    private void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            settings.animator.SetBool("Twerk", EnemyManager.instance.isRadioOn);

            if (canAttack)
            {
                AttackCheck();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void OnDeath(Collider collider)
    {
        damagedAudio.pitch = Random.Range(0.75f, 1.1f);
        damagedAudio.Play();

        Instantiate(explosion, bombCollider.transform.position, bombCollider.transform.rotation);

        Collider[] hitColliders = Physics.OverlapSphere(bombCollider.transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.TryGetComponent(out IDamagable damagable))
            {
                
                if(!hit.CompareTag("Player"))
                damagable.Damage(450, GetComponent<Collider>(), 0);

                else
                {
                    PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                    if(!playerHealth.isImmuneToExplosions)
                    damagable.Damage(hit.GetComponent<PlayerHealth>().currentHp / 2, GetComponent<Collider>(), 0);

                    else
                    damagable.Damage(0, GetComponent<Collider>(), 0);
                }
            }
        }

        Destroy(gameObject);
    }
    

    private void AttackCheck()
    {
        if (!canAttack) return;

        facePos = transform.position + transform.forward * attackRange;
        Collider[] scannedColliders = Physics.OverlapSphere(facePos, attackRange, settings.playerLayer);

        if (scannedColliders.Length > 0)
        {
            Attack(scannedColliders);
        }
    }

    private void Attack(Collider[] colliders)
    {
        foreach (Collider player in colliders)
        {
            if (player.TryGetComponent(out IDamagable damagable))
            {
                damagable.Damage(attackDamage, settings.bodyCollider, attackKnockback);
            }
        }

        canAttack = false;
        Invoke(nameof(PrepareAttack), attackDelay);
    }

    private void PrepareAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bombCollider.transform.position, explosionRadius);
    }
}
